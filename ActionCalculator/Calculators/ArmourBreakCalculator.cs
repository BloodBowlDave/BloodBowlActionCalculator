using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Calculators
{
    public class ArmourBreakCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly ITwoD6 _twoD6;
        private const Skills SkillsAffectingArmour = Skills.Ram | Skills.MightyBlow | Skills.Slayer | Skills.CrushingBlow;

        public ArmourBreakCalculator(ICalculator calculator, ITwoD6 twoD6)
        {
            _calculator = calculator;
            _twoD6 = twoD6;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;

            var armourRoll = action.OriginalRoll;
            var useOldPro = player.HasSkill(Skills.OldPro) && !usedSkills.Contains(Skills.Pro);

            if (player.HasSkill(Skills.Claw) && armourRoll >= 8)
            {
                var success = _twoD6.Success(8);
                _calculator.Calculate(p * success, r, playerAction, usedSkills);

                if (!useOldPro)
                {
                    return;
                }
                
                var successWithOldPro = (decimal)GetSuccessesWithOldPro(new Dictionary<Skills, int> { { Skills.None, 8 } })[Skills.None] / 216;
                _calculator.Calculate(p * (1 - success) * player.ProSuccess * successWithOldPro, r, playerAction, usedSkills | Skills.Pro);

                return;
            }

            var skillsWithMinimumRoll = GetSkillsWithMinimumRoll(player, armourRoll);
            var succeedWithPreviousSkills = 0m;

            foreach (var (skills, minimumRoll) in skillsWithMinimumRoll)
            {
                var success = _twoD6.Success(minimumRoll) - succeedWithPreviousSkills;
                _calculator.Calculate(p * success, r, playerAction, usedSkills | skills);
                succeedWithPreviousSkills += success;
            }

            if (!useOldPro)
            {
                return;
            }
            
            foreach (var (skills, successes) in GetSuccessesWithOldPro(skillsWithMinimumRoll))
            {
                _calculator.Calculate(p * player.ProSuccess * successes / 216, r, playerAction, usedSkills | skills | Skills.Pro);
            }
        }

        private Dictionary<Skills, int> GetSuccessesWithOldPro(Dictionary<Skills, int> skillsWithMinimumRoll)
        {
            var successesWithOldPro = skillsWithMinimumRoll.ToDictionary(x => x.Key, _ => 0);

            var lowestSuccessfulArmourRoll = skillsWithMinimumRoll.Last().Value;

            for (var roll = lowestSuccessfulArmourRoll - 1; roll >= 2; roll--)
            {
                var combinations = _twoD6.GetCombinationsForRoll(roll);
                var rollHasSuccessfulCombination = false;

                foreach (var highestRoll in combinations.Select(x => x.Item1.ThisOrMinimum(x.Item2)))
                {
                    for (var newRoll = 6; newRoll >= 2; newRoll--)
                    {
                        var rerolledArmourRoll = newRoll + highestRoll;
                        if (rerolledArmourRoll < lowestSuccessfulArmourRoll)
                        {
                            break;
                        }

                        rollHasSuccessfulCombination = true;

                        var skills = skillsWithMinimumRoll.First(x => x.Value <= rerolledArmourRoll).Key;
                    
                        successesWithOldPro[skills]++;
                    }
                }

                if (!rollHasSuccessfulCombination)
                {
                    break;
                }
            }
            
            return successesWithOldPro;
        }

        private static Dictionary<Skills, int> GetSkillsWithMinimumRoll(Player player, int armourRoll)
        {
            var previousMinimumRoll = 99;
            var skillsWithMinimumRoll = new Dictionary<Skills, int>();

            foreach (var skillCombination in GetSkillCombinations(player))
            {
                var modifier = skillCombination.Sum(x => x.Item2);
                var minimumRoll = armourRoll - modifier;

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

        private static IEnumerable<Tuple<Skills, int>[]> GetSkillCombinations(Player player) =>
            SkillsAffectingArmour.ToEnumerable(Skills.None)
                .Where(x => player.HasSkill(x))
                .Select(x => new Tuple<Skills, int>(x, x == Skills.MightyBlow ? player.MightyBlowValue : 1))
                .Combinations()
                .OrderBy(x => x.Sum(y => y.Item2))
                .ThenBy(x => !x.Select(y => y.Item1).Contains(Skills.CrushingBlow));
    }
}