using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Strategies.Blocking
{
    public class TwoDiceBlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly IBrawlerHelper _brawlerHelper;

        public TwoDiceBlockStrategy(IActionMediator actionMediator,
            IProHelper proHelper, IBrawlerHelper brawlerHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _brawlerHelper = brawlerHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, _), action, i) = playerAction;
            var (success, failure) = action;
            var oneDieSuccess = action.SuccessOnOneDie;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            var failAndUseBrawler = 0m;

            if (_brawlerHelper.UseBrawler(r, playerAction, usedSkills))
            {
                failAndUseBrawler = _brawlerHelper.ProbabilityCanUseBrawler(action);
                _actionMediator.Resolve(p * failAndUseBrawler * oneDieSuccess, r, i, usedSkills);

                var pBrawler = p * failAndUseBrawler * (1 - oneDieSuccess) * oneDieSuccess;

                if (_proHelper.UsePro(playerAction, r, usedSkills, oneDieSuccess, success))
                {
                    _actionMediator.Resolve(pBrawler * proSuccess, r, i, usedSkills | Skills.Pro);
                    return;
                }

                if (r > 0)
                {
                    _actionMediator.Resolve(pBrawler * rerollSuccess, r - 1, i, usedSkills);
                    return;
                }
            }

            p *= failure - failAndUseBrawler;

            if (_proHelper.UsePro(playerAction, r, usedSkills, oneDieSuccess, success))
            {
                usedSkills |= Skills.Pro;
                _actionMediator.Resolve(p * proSuccess * oneDieSuccess, r, i, usedSkills);

                if (r > 0)
                {
                    _actionMediator.Resolve(p * (1 - proSuccess * oneDieSuccess) * rerollSuccess * oneDieSuccess, r - 1, i, usedSkills);
                    return;
                }
            }

            if (r > 0)
            {
                _actionMediator.Resolve(p * rerollSuccess * success, r - 1, i, usedSkills);
            }
        }
    }
}
