using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class ArmourBreakCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly ITwoD6 _twoD6;

        public ArmourBreakCalculator(ICalculator calculator, ITwoD6 twoD6)
        {
            _calculator = calculator;
            _twoD6 = twoD6;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var success = action.Success;

            var useClaw = player.HasSkill(Skills.Claw) && action.OriginalRoll >= 8;
            if (useClaw)
            {
                success = _twoD6.Success(8);
            }

            var useMbOnArmour = 0m;
            var hasMightyBlow = player.HasSkill(Skills.MightyBlow);
            if (hasMightyBlow && !useClaw)
            {
                useMbOnArmour = _twoD6.Success(action.OriginalRoll - 1) - success;
            }
        
            _calculator.Calculate(p * success, r, playerAction, usedSkills);
            _calculator.Calculate(p * useMbOnArmour, r, playerAction, usedSkills | Skills.MightyBlow);
        }
    }
}