using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class RerollableActionCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public RerollableActionCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
	        _calculator = calculator;
	        _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var success = playerAction.Action.Success;
            var failure = playerAction.Action.Failure;

            _calculator.Calculate(p * success, r, playerAction, usedSkills);

            p *= failure * success;

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
	            _calculator.Calculate(p * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro);
	            return;
            }

            if (r > 0)
            {
	            _calculator.Calculate(p * player.LonerSuccess, r - 1, playerAction, usedSkills);
            }
        }
    }
}