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

            _calculator.Calculate(p * action.Success, r, playerAction, usedSkills);

            p *= action.Failure;

            if (action.RerollNonCriticalFailure)
            {
                if (_proCalculator.UsePro(playerAction, r, usedSkills))
                {
                    _calculator.Calculate(p * player.ProSuccess * action.Success, r, playerAction, usedSkills | Skills.Pro);
                    _calculator.Calculate(p * (player.ProSuccess * action.Failure + (1 - player.ProSuccess)), r, playerAction, usedSkills | Skills.Pro, true);

                    return;
                }

                if (r > 0)
                {
                    _calculator.Calculate(p * player.LonerSuccess * action.Success, r - 1, playerAction, usedSkills);
                    _calculator.Calculate(p * player.LonerSuccess * action.Failure, r - 1, playerAction, usedSkills, true);

                    return;
                }
            }

            if (action.UsePro && _proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(p * player.ProSuccess * action.Success, r, playerAction, usedSkills | Skills.Pro);
                _calculator.Calculate(p * (player.ProSuccess * action.Failure + (1 - player.ProSuccess)), r, playerAction, usedSkills | Skills.Pro, true);

                return;
            }

            _calculator.Calculate(p, r, playerAction, usedSkills, true);
        }
    }
}