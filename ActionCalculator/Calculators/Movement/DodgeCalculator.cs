using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Movement
{
    public class DodgeCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public DodgeCalculator(ICalculator calculator, IProCalculator proCalculator)
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

            if (player.HasSkill(Skills.Dodge) && !usedSkills.HasFlag(Skills.Dodge))
            {
                _calculator.Calculate(p, r, playerAction, usedSkills | Skills.Dodge);
                return;
            }

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