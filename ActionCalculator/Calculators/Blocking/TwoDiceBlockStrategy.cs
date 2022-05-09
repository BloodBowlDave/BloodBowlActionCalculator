using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Calculators.Blocking
{
    public class TwoDiceBlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProCalculator _proCalculator;
        private readonly IBrawlerCalculator _brawlerCalculator;

        public TwoDiceBlockStrategy(IActionMediator actionMediator,
            IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
        {
            _actionMediator = actionMediator;
            _proCalculator = proCalculator;
            _brawlerCalculator = brawlerCalculator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, _), action, i) = playerAction;
            var (success, failure) = action;
            var oneDieSuccess = action.SuccessOnOneDie;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            var failAndUseBrawler = 0m;

            if (_brawlerCalculator.UseBrawler(r, playerAction, usedSkills))
            {
                failAndUseBrawler = _brawlerCalculator.ProbabilityCanUseBrawler(action);
                _actionMediator.Resolve(p * failAndUseBrawler * oneDieSuccess, r, i, usedSkills);

                var pBrawler = p * failAndUseBrawler * (1 - oneDieSuccess) * oneDieSuccess;

                if (_proCalculator.UsePro(playerAction, r, usedSkills, oneDieSuccess, success))
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

            if (_proCalculator.UsePro(playerAction, r, usedSkills, oneDieSuccess, success))
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
