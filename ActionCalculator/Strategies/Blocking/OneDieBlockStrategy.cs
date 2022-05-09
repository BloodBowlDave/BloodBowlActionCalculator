using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Strategies.Blocking
{
    public class OneDieBlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly IBrawlerHelper _brawlerHelper;

        public OneDieBlockStrategy(IActionMediator actionMediator,
            IProHelper proHelper, IBrawlerHelper brawlerHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _brawlerHelper = brawlerHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, _), action, i) = playerAction;
            var success = action.Success;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            var failButRollBothDown = 0m;

            if (_brawlerHelper.UseBrawler(r, playerAction, usedSkills))
            {
                failButRollBothDown = _brawlerHelper.ProbabilityCanUseBrawler(action);
                _actionMediator.Resolve(p * failButRollBothDown * success, r, i, usedSkills);
            }

            p *= (action.Failure - failButRollBothDown) * success;

            if (_proHelper.UsePro(playerAction, r, usedSkills))
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
