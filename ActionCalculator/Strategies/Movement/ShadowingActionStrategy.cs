using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.Movement
{
    public class ShadowingActionStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public ShadowingActionStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, _) = player;
            var action = (Shadowing) playerAction.Action;
            var success = action.Success;
            var failure = action.Failure;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= action.Failure;

            if (_proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess, success, failure);
                return;
            }

            if (r > 0 && action.RerollFailure)
            {
                ExecuteReroll(p, r - 1, i, usedSkills, lonerSuccess, success, failure);
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal rerollSuccess, decimal success, decimal failure)
        {
            _actionMediator.Resolve(p * rerollSuccess * success, r, i, usedSkills);
            _actionMediator.Resolve(p * (1 - rerollSuccess + rerollSuccess * failure), r, i, usedSkills, true);
        }
    }
}