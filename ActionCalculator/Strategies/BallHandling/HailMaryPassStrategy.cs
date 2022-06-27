using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.BallHandling
{
    public class HailMaryPassStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public HailMaryPassStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var hailMaryPass = (HailMaryPass) playerAction.Action;
            var success = hailMaryPass.Success;
            var failure = hailMaryPass.Failure;
            var i = playerAction.Index;

            if (canUseSkill(Skills.BlastIt, usedSkills))
            {
                usedSkills |= Skills.BlastIt;
            }

            _actionMediator.Resolve(p * success, r, i, usedSkills, true);

            if (_proHelper.UsePro(player, hailMaryPass, r, usedSkills, success, success))
            {
                _actionMediator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | Skills.Pro, true);
                return;
            }
            
            _actionMediator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills, true);
        }
    }
}