using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Fouling
{
    public class ArgueTheCallStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;

        public ArgueTheCallStrategy(IActionMediator actionMediator)
        {
            _actionMediator = actionMediator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool doubleOnFoul = false)
        {
            var argueTheCall = (ArgueTheCall) playerAction.Action;
            var i = playerAction.Index;

            if (doubleOnFoul)
            {
                _actionMediator.Resolve(p * argueTheCall.Success, r, i, usedSkills);
                _actionMediator.Resolve(p * argueTheCall.Failure, r, i, usedSkills, true);
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills);
        }
    }
}