using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class ArmourBreakStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _iD6;
        private const Skills SkillsAffectingArmour = Skills.Ram | Skills.MightyBlow | Skills.Slayer | Skills.CrushingBlow;

        public ArmourBreakStrategy(IActionMediator actionMediator, ID6 iD6)
        {
            _actionMediator = actionMediator;
            _iD6 = iD6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (_, proSuccess, canUseSkill) = player;
            var armourBreak = (ArmourBreak) playerAction.Action;
            var i = playerAction.Index;

            var roll = armourBreak.Numerator;
            var useOldPro = canUseSkill(Skills.OldPro, usedSkills);

            if (canUseSkill(Skills.Claw, usedSkills) && roll >= 8)
            {
                var success = _iD6.Success(2, 8);
                _actionMediator.Resolve(p * success, r, i, usedSkills);

                if (!useOldPro)
                {
                    return;
                }

                var successesWithOldPro = GetSuccessesWithOldPro(new Dictionary<Skills, int> { { Skills.None, 8 } });
                var successWithOldPro = (decimal)successesWithOldPro[Skills.None] / 216;
                _actionMediator.Resolve(p * (1 - success) * proSuccess * successWithOldPro, r, i, usedSkills | Skills.Pro);

                return;
            }

            var skillsWithMinimumRoll = GetSkillsWithMinimumRoll(player, usedSkills, roll);
            var succeedWithPreviousSkills = 0m;

            foreach (var (skills, minimumRoll) in skillsWithMinimumRoll)
            {
                var success = _iD6.Success(2, minimumRoll) - succeedWithPreviousSkills;
                _actionMediator.Resolve(p * success, r, i, usedSkills | skills);
                succeedWithPreviousSkills += success;
            }

            if (!useOldPro)
            {
                return;
            }
            
            foreach (var (skills, successes) in GetSuccessesWithOldPro(skillsWithMinimumRoll))
            {
                _actionMediator.Resolve(p * proSuccess * successes / 216, r, i, usedSkills | skills | Skills.Pro);
            }
        }

        private Dictionary<Skills, int> GetSuccessesWithOldPro(Dictionary<Skills, int> skillsWithMinimumRoll)
        {
            var successesWithOldPro = skillsWithMinimumRoll.ToDictionary(x => x.Key, _ => 0);

            var lowestSuccessfulArmourRoll = skillsWithMinimumRoll.Last().Value;
            
            foreach (var roll in _iD6.Rolls(2).Where(x => x.Sum() < lowestSuccessfulArmourRoll))
            {
                var highestRoll = roll.Max();
            
                for (var newRoll = 6; newRoll >= 2; newRoll--)
                {
                    var rerolledArmourRoll = newRoll + highestRoll;
                    if (rerolledArmourRoll < lowestSuccessfulArmourRoll)
                    {
                        break;
                    }
                    
                    var skills = skillsWithMinimumRoll.First(x => x.Value <= rerolledArmourRoll).Key;
                
                    successesWithOldPro[skills]++;
                }
            }
            
            return successesWithOldPro;
        }

        private static Dictionary<Skills, int> GetSkillsWithMinimumRoll(Player player, Skills usedSkills, int roll)
        {
            var previousMinimumRoll = 99;
            var skillsWithMinimumRoll = new Dictionary<Skills, int>();

            foreach (var skillCombination in GetSkillCombinations(player, usedSkills))
            {
                var modifier = skillCombination.Sum(x => x.Item2);
                var minimumRoll = roll - modifier;

                if (minimumRoll >= previousMinimumRoll)
                {
                    continue;
                }

                var skills = skillCombination.Select(x => x.Item1)
                    .Aggregate(Skills.None, (current, skill) =>
                        current | (skill == Skills.CrushingBlow ? Skills.None : skill));

                if (skillsWithMinimumRoll.ContainsKey(skills))
                {
                    skillsWithMinimumRoll[skills] = minimumRoll;
                }
                else
                {
                    skillsWithMinimumRoll.Add(skills, minimumRoll);
                }

                previousMinimumRoll = minimumRoll;
            }

            return skillsWithMinimumRoll;
        }

        private static IEnumerable<Tuple<Skills, int>[]> GetSkillCombinations(Player player, Skills useSkills) =>
            SkillsAffectingArmour.ToEnumerable(Skills.None)
                .Where(x => player.CanUseSkill(x, useSkills))
                .Select(x => new Tuple<Skills, int>(x, x == Skills.MightyBlow ? player.MightyBlowValue : 1))
                .Combinations()
                .OrderBy(x => x.Sum(y => y.Item2))
                .ThenBy(x => !x.Select(y => y.Item1).Contains(Skills.CrushingBlow));
    }
}