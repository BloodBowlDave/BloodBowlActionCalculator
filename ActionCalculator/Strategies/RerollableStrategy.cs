using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies
{
    public class RerollableStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public RerollableStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, _) = player;
            var action = (RerollableAction) playerAction.Action;
            var success = action.Success;
            var failure = action.Failure;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure * success;

            if (_proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _actionMediator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}