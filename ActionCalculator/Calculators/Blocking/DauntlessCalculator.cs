using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Blocking
{
    public class DauntlessCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public DauntlessCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var success = action.Success;

            _calculator.Calculate(p * success, r, playerAction, usedSkills);

            p *= action.Failure;

            if (action.RerollNonCriticalFailure)
            {
                if (_proCalculator.UsePro(playerAction, r, usedSkills))
                {
                    usedSkills |= Skills.Pro;
                    _calculator.Calculate(p * player.ProSuccess * success, r, playerAction, usedSkills);
                    _calculator.Calculate(p * (player.ProSuccess * action.Failure + (1 - player.ProSuccess)), r, playerAction, usedSkills, true);

                    return;
                }

                if (r > 0)
                {
                    _calculator.Calculate(p * player.UseReroll * success, r - 1, playerAction, usedSkills);
                    _calculator.Calculate(p * player.UseReroll * action.Failure, r - 1, playerAction, usedSkills, true);

                    return;
                }
            }

            if (action.UsePro && _proCalculator.UsePro(playerAction, r, usedSkills))
            {
                usedSkills |= Skills.Pro;
                _calculator.Calculate(p * player.ProSuccess * success, r, playerAction, usedSkills);
                _calculator.Calculate(p * (player.ProSuccess * action.Failure + (1 - player.ProSuccess)), r, playerAction, usedSkills, true);

                return;
            }

            _calculator.Calculate(p, r, playerAction, usedSkills, true);
        }
    }
}