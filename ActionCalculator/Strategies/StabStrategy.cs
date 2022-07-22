using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
    public class StabStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _d6;
        
        public StabStrategy(IActionMediator actionMediator, ID6 d6)
        {
            _actionMediator = actionMediator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var action = playerAction.Action;
            var success = _d6.Success(2, action.Numerator);

            _actionMediator.Resolve(p * success, r, i, usedSkills);
            _actionMediator.Resolve(p * (1 - success), r, i, usedSkills, true);
        }
    }
}
