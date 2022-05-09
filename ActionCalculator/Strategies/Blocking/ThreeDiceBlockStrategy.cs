using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Strategies.Blocking
{
    public class ThreeDiceBlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly IBrawlerHelper _brawlerHelper;

        public ThreeDiceBlockStrategy(IActionMediator actionMediator, IProHelper proHelper, IBrawlerHelper brawlerHelper)
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

            var failButRollBothDown = 0m;

            if (_brawlerHelper.CanUseBrawler(r, playerAction, usedSkills))
            {
                failButRollBothDown = _brawlerHelper.UseBrawler(action);
                _actionMediator.Resolve(p * failButRollBothDown * oneDieSuccess, r, i, usedSkills);

                var pBrawler = p * failButRollBothDown * (1 - oneDieSuccess);

                if (_proHelper.CanUsePro(playerAction, r, usedSkills, oneDieSuccess, success))
                {
                    usedSkills |= Skills.Pro;
                    _actionMediator.Resolve(pBrawler * proSuccess * oneDieSuccess, r, i, usedSkills);

                    if (r == 0)
                    {
                        return;
                    }

                    _actionMediator.Resolve(pBrawler * (1 - proSuccess * oneDieSuccess) * rerollSuccess * oneDieSuccess, r - 1, i, usedSkills);
                    return;
                }

                if (r > 0)
                {
                    _actionMediator.Resolve(pBrawler * rerollSuccess * action.SuccessOnTwoDice, r - 1, i, usedSkills);
                    return;
                }
            }

            p *= failure - failButRollBothDown;

            if (_proHelper.CanUsePro(playerAction, r, usedSkills, oneDieSuccess, success))
            {
                usedSkills |= Skills.Pro;
                _actionMediator.Resolve(p * proSuccess * oneDieSuccess, r, i, usedSkills);

                if (r > 0)
                {
                    _actionMediator.Resolve(p * (1 - proSuccess * oneDieSuccess) * rerollSuccess * action.SuccessOnTwoDice, r - 1, i, usedSkills);
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
