using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.BallHandling
{
    public class CatchStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public CatchStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var @catch = (Catch) playerAction.Action;
            var success = @catch.Success;
            var failure = @catch.Failure;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= success * failure;

            if (canUseSkill(Skills.Catch, usedSkills))
            {
                _actionMediator.Resolve(p, r, i, usedSkills);
                return;
            }

            if (_proHelper.UsePro(player, @catch, r, usedSkills, success, success))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _actionMediator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}