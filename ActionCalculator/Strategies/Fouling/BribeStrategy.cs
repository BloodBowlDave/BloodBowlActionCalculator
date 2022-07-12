using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.Fouling
{
    public class BribeStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _d6;

        public BribeStrategy(IActionMediator actionMediator, ID6 d6)
        {
            _actionMediator = actionMediator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var success = _d6.Success(1, 2);
            var failure = 1 - success;
            var i = playerAction.Index;

            if (nonCriticalFailure)
            {
                _actionMediator.Resolve(p * success, r, i, usedSkills);
                _actionMediator.Resolve(p * failure, r, i, usedSkills, true);
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills);
        }
    }
}