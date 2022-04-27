using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Movement
{
    public class TentaclesCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public TentaclesCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var action = playerAction.Action;
            var player = playerAction.Player;
            var success = action.Success;

            _calculator.Calculate(p * success, r, playerAction, usedSkills);

            p *= action.Failure;

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                usedSkills |= Skills.Pro;
                _calculator.Calculate(p * player.ProSuccess * success, r, playerAction, usedSkills);
                _calculator.Calculate(p * (1 - player.ProSuccess + player.ProSuccess * action.Failure), r, playerAction, usedSkills, true);
                return;
            }

            if (r > 0 && action.RerollNonCriticalFailure)
            {
                _calculator.Calculate(p * player.UseReroll * success, r - 1, playerAction, usedSkills);
                _calculator.Calculate(p * (1 - player.UseReroll + player.UseReroll * action.Failure), r - 1, playerAction, usedSkills, true);
                return;
            }

            _calculator.Calculate(p, r, playerAction, usedSkills, true);
        }
    }
}