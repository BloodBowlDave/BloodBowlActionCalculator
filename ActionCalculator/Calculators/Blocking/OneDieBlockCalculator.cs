using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Calculators.Blocking
{
    public class OneDieBlockCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;
        private readonly IBrawlerCalculator _brawlerCalculator;

        public OneDieBlockCalculator(ICalculator calculator,
            IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
            _brawlerCalculator = brawlerCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var success = action.Success;

            _calculator.Calculate(p * success, r, playerAction, usedSkills);

            var failButRollBothDown = 0m;

            if (_brawlerCalculator.UseBrawler(r, playerAction))
            {
                failButRollBothDown = _brawlerCalculator.ProbabilityCanUseBrawler(action);
                _calculator.Calculate(p * failButRollBothDown * success, r, playerAction, usedSkills);
            }

            p *= (action.Failure - failButRollBothDown) * success;

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(p * player.ProSuccess, r, playerAction, usedSkills);
                return;
            }

            if (r > 0)
            {
                _calculator.Calculate(p * player.UseReroll, r - 1, playerAction, usedSkills);
            }
        }
    }
}
