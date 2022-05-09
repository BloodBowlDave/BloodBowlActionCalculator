using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Fouling
{
    public class BribeStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;

        public BribeStrategy(IActionMediator actionMediator)
        {
            _actionMediator = actionMediator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (_, (success, failure), i) = playerAction;

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