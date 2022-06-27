using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Blocking
{
    public class DauntlessStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public DauntlessStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var dauntless = (Dauntless) playerAction.Action;
            var success = dauntless.Success;
            var failure = dauntless.Failure;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * success, r, i, usedSkills);
            
            p *= failure;

            if (dauntless.RerollFailure)
            {
                if (canUseSkill(Skills.BlindRage, usedSkills))
                {
                    ExecuteReroll(p, r, i, usedSkills | Skills.BlindRage, success, failure);
                    return;
                }

                if (_proHelper.UsePro(player, dauntless, r, usedSkills, success, success))
                {
                    ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess * success, proSuccess * failure + (1 - proSuccess));
                    return;
                }

                if (r > 0)
                {
                    _actionMediator.Resolve(p * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                    ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills, success, failure);
                    return;
                }
            }

            if (_proHelper.UsePro(player, dauntless, r, usedSkills, success, success))
            {
                ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess * success, proSuccess * failure + (1 - proSuccess));
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal success, decimal failure)
        {
            _actionMediator.Resolve(p * success, r, i, usedSkills);
            _actionMediator.Resolve(p * failure, r, i, usedSkills, true);
        }
    }
}