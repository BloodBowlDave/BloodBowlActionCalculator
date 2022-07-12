using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.Movement
{
    public class RushStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public RushStrategy(IActionMediator actionMediator, IProHelper proHelper, ID6 d6)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var rush = playerAction.Action;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var success = _d6.Success(1, rush.Numerator);
            var failure = 1 - success;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure * success;

            if (canUseSkill(Skills.SureFeet, usedSkills))
            {
                _actionMediator.Resolve(p, r, i, usedSkills | Skills.SureFeet);
                return;
            }

            if (_proHelper.UsePro(player, rush, r, usedSkills, success, success))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _actionMediator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}