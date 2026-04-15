using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class ArmourBreakStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _iD6;
        private const CalculatorSkills SkillsAffectingArmour = CalculatorSkills.Ram | CalculatorSkills.MightyBlow | CalculatorSkills.Slayer | CalculatorSkills.CrushingBlow;

        public ArmourBreakStrategy(ICalculator calculator, ID6 iD6)
        {
            _calculator = calculator;
            _iD6 = iD6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (_, proSuccess, canUseSkill) = player;
            var armourBreak = (ArmourBreak) playerAction.Action;

            var roll = armourBreak.Roll;
            var useOldPro = canUseSkill(CalculatorSkills.OldPro, usedSkills);

            if (canUseSkill(CalculatorSkills.Claw, usedSkills) && roll >= 8)
            {
                var success = _iD6.Success(2, 8);
                _calculator.Resolve(p * success, r, i, usedSkills);

                if (!useOldPro)
                {
                    return;
                }

                var successesWithOldPro = GetSuccessesWithOldPro(new Dictionary<CalculatorSkills, int> { { CalculatorSkills.None, 8 } });
                var successWithOldPro = (decimal)successesWithOldPro[CalculatorSkills.None] / 216;
                _calculator.Resolve(p * (1 - success) * proSuccess * successWithOldPro, r, i, usedSkills | CalculatorSkills.Pro);

                return;
            }

            var skillsWithMinimumRoll = GetSkillsWithMinimumRoll(player, usedSkills, roll);
            var succeedWithPreviousSkills = 0m;

            foreach (var (skills, minimumRoll) in skillsWithMinimumRoll)
            {
                var success = _iD6.Success(2, minimumRoll) - succeedWithPreviousSkills;
                _calculator.Resolve(p * success, r, i, usedSkills | skills);
                succeedWithPreviousSkills += success;
            }

            if (!useOldPro)
            {
                return;
            }

            foreach (var (skills, successes) in GetSuccessesWithOldPro(skillsWithMinimumRoll))
            {
                _calculator.Resolve(p * proSuccess * successes / 216, r, i, usedSkills | skills | CalculatorSkills.Pro);
            }
        }

        private Dictionary<CalculatorSkills, int> GetSuccessesWithOldPro(Dictionary<CalculatorSkills, int> skillsWithMinimumRoll)
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

        private static Dictionary<CalculatorSkills, int> GetSkillsWithMinimumRoll(Player player, CalculatorSkills usedSkills, int roll)
        {
            var previousMinimumRoll = 99;
            var skillsWithMinimumRoll = new Dictionary<CalculatorSkills, int>();

            foreach (var skillCombination in GetSkillCombinations(player, usedSkills))
            {
                var modifier = skillCombination.Sum(x => x.Item2);
                var minimumRoll = roll - modifier;

                if (minimumRoll >= previousMinimumRoll)
                {
                    continue;
                }

                var skills = skillCombination.Select(x => x.Item1)
                    .Aggregate(CalculatorSkills.None, (current, skill) =>
                        current | (skill == CalculatorSkills.CrushingBlow ? CalculatorSkills.None : skill));

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

        private static IEnumerable<Tuple<CalculatorSkills, int>[]> GetSkillCombinations(Player player, CalculatorSkills useSkills) =>
            SkillsAffectingArmour.ToEnumerable(CalculatorSkills.None)
                .Where(x => player.CanUseSkill(x, useSkills))
                .Select(x => new Tuple<CalculatorSkills, int>(x, x == CalculatorSkills.MightyBlow ? player.MightyBlowValue : 1))
                .Combinations()
                .OrderBy(x => x.Sum(y => y.Item2))
                .ThenBy(x => !x.Select(y => y.Item1).Contains(CalculatorSkills.CrushingBlow));
    }
}