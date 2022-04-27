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
            if (player.HasSkill(Skills.Claw) && armourRoll >= 8)
            {
                _calculator.Calculate(p * _twoD6.Success(8), r, playerAction, usedSkills);
                return;
            }
            
            var skillsWithSuccessValue = GetSkillsWithSuccessValue(player, armourRoll);

            foreach (var (skills, success) in skillsWithSuccessValue)
            {
                _calculator.Calculate(p * success, r, playerAction, usedSkills | skills);
            }
        }

        private Dictionary<Skills, decimal> GetSkillsWithSuccessValue(Player player, int armourRoll)
        {
            var successUsingPreviousSkillCombination = 0m;
            var skillsWithSuccessValue = new Dictionary<Skills, decimal>();

            foreach (var skillCombination in GetSkillCombinations(player))
            {
                var modifier = skillCombination.Sum(x => x.Item2);
                var successWithSkillCombination = _twoD6.Success(armourRoll - modifier) - successUsingPreviousSkillCombination;

                if (successWithSkillCombination <= 0)
                {
                    continue;
                }

                var skills = skillCombination.Select(x => x.Item1)
                    .Aggregate(Skills.None, (current, skill) => 
                        current | (skill == Skills.CrushingBlow ? Skills.None : skill));

                if (skillsWithSuccessValue.ContainsKey(skills))
                {
                    skillsWithSuccessValue[skills] += successWithSkillCombination;
                }
                else
                {
                    skillsWithSuccessValue.Add(skills, successWithSkillCombination);
                }

                successUsingPreviousSkillCombination += successWithSkillCombination;
            }

            return skillsWithSuccessValue;
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