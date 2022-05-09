using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Fouling
{
    public class ArgueTheCallStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;

        public ArgueTheCallStrategy(IActionMediator actionMediator)
        {
            _actionMediator = actionMediator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (_, action, i) = playerAction;

            if (nonCriticalFailure)
            {
                _actionMediator.Resolve(p * action.Success, r, i, usedSkills);
                _actionMediator.Resolve(p * action.NonCriticalFailure, r, i, usedSkills, true);
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills);
        }
    }
}