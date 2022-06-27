using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Fouling
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
            var bribe = (Bribe) playerAction.Action;
            var success = bribe.Success;
            var failure = bribe.Failure;
            var i = playerAction.Index;

            if (nonCriticalFailure)
            {
                _actionMediator.Resolve(p * success, r, i, usedSkills);
                _actionMediator.Resolve(p * failure, r, i, usedSkills, true);
                _actionMediator.SendOff(p * failure, r, i);
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills);
        }
    }
}