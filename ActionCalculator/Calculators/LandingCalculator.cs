using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class LandingCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public LandingCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }
        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var action = playerAction.Action;
            var player = playerAction.Player;
            var success = nonCriticalFailure ? (7m - (action.OriginalRoll + 1).ThisOrMinimum(2).ThisOrMaximum(6)) / 6 : action.Success;
            var failure = 1m - success;
            
            _calculator.Calculate(p * success, r, playerAction, usedSkills);

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(p * failure * player.ProSuccess * success, r, playerAction, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _calculator.Calculate(p * failure * player.LonerSuccess * success, r - 1, playerAction, usedSkills);
            }
        }
    }
}
