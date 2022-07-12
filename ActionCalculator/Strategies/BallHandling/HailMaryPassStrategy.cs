using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class HailMaryPassStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public HailMaryPassStrategy(IActionMediator actionMediator, IProHelper proHelper, ID6 d6)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var hailMaryPass = playerAction.Action;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var success = _d6.Success(1, hailMaryPass.Numerator);
            var failure = 1 - success;
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