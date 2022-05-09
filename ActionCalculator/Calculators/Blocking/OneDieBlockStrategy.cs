using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Calculators.Blocking
{
    public class OneDieBlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProCalculator _proCalculator;
        private readonly IBrawlerCalculator _brawlerCalculator;

        public OneDieBlockStrategy(IActionMediator actionMediator,
            IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
        {
            _actionMediator = actionMediator;
            _proCalculator = proCalculator;
            _brawlerCalculator = brawlerCalculator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, _), action, i) = playerAction;
            var success = action.Success;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            var failButRollBothDown = 0m;

            if (_brawlerCalculator.UseBrawler(r, playerAction, usedSkills))
            {
                failButRollBothDown = _brawlerCalculator.ProbabilityCanUseBrawler(action);
                _actionMediator.Resolve(p * failButRollBothDown * success, r, i, usedSkills);
            }

            p *= (action.Failure - failButRollBothDown) * success;

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills);
                return;
            }

            if (r > 0)
            {
                _actionMediator.Resolve(p * rerollSuccess, r - 1, i, usedSkills);
            }
        }
    }
}
