using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

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
            var ((rerollSuccess, proSuccess, canUseSkill), action, i) = playerAction;
            var (success, failure) = action;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure;

            if (action.RerollNonCriticalFailure)
            {
                if (canUseSkill(Skills.BlindRage, usedSkills))
                {
                    ExecuteReroll(p, r, i, usedSkills | Skills.BlindRage, success, failure);
                    return;
                }

                if (_proHelper.CanUsePro(playerAction, r, usedSkills))
                {
                    ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess * success, proSuccess * failure + (1 - proSuccess));
                    return;
                }

                if (r > 0)
                {
                    ExecuteReroll(p * rerollSuccess, r - 1, i, usedSkills, success, failure);
                    return;
                }
            }

            if (action.UsePro && _proHelper.CanUsePro(playerAction, r, usedSkills))
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