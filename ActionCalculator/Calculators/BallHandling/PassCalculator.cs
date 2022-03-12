using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.BallHandling
{
    public class PassCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public PassCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
	        _calculator = calculator;
	        _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;

            _calculator.Calculate(p * action.Success, r, playerAction, usedSkills);

            var accuratePassAfterFailure = p * (action.Failure + action.NonCriticalFailure) * action.Success;
            var inaccuratePassAfterFailure = p * (action.Failure + action.NonCriticalFailure) * action.NonCriticalFailure;

            if (player.HasSkill(Skills.Pass))
            {
                _calculator.Calculate(accuratePassAfterFailure, r, playerAction, usedSkills);
                _calculator.Calculate(inaccuratePassAfterFailure, r, playerAction, usedSkills, true);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
	            _calculator.Calculate(player.ProSuccess * accuratePassAfterFailure, r, playerAction, usedSkills | Skills.Pro);
	            _calculator.Calculate(player.ProSuccess * inaccuratePassAfterFailure, r, playerAction, usedSkills | Skills.Pro, true);
                return;
            }

            if (r > 0)
            {
	            _calculator.Calculate(player.LonerSuccess * accuratePassAfterFailure, r - 1, playerAction, usedSkills);
	            _calculator.Calculate(player.LonerSuccess * inaccuratePassAfterFailure, r - 1, playerAction, usedSkills, true);
	            return;
            }

            _calculator.Calculate(p * action.NonCriticalFailure, r, playerAction, usedSkills, true);
        }
    }
}