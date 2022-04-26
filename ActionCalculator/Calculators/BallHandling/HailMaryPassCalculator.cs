using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.BallHandling
{
    public class HailMaryPassCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public HailMaryPassCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var success = action.Success;
            var failure = action.Failure;
            
            _calculator.Calculate(p * success, r, playerAction, usedSkills, true);

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(p * failure * player.ProSuccess * success, r, playerAction, usedSkills | Skills.Pro, true);
                return;
            }

            if (r == 0)
            {
                return;
            }

            _calculator.Calculate(p * failure * player.UseReroll * success, r - 1, playerAction, usedSkills, true);
        }
    }
}