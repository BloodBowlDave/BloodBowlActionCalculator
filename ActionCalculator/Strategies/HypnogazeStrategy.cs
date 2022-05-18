using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies
{
    public class HypnogazeStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public HypnogazeStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure)
        {
            var ((lonerSuccess, proSuccess, canUseSkill), action, i) = playerAction;
            var (success, failure) = action;

            _actionMediator.Resolve(p * action.Success, r, i, usedSkills);

            p *= action.Failure;

            if (canUseSkill(Skills.MesmerisingDance, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills, success, failure);
            }

            if (_proHelper.UsePro(playerAction, r, usedSkills))
            {
                ExecuteReroll(p * proSuccess, r, i, usedSkills | Skills.Pro, success, failure);
                return;
            }

            if (r > 0 && action.RerollNonCriticalFailure)
            {
                ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills, success, failure);
                _actionMediator.Resolve(p * (1 - lonerSuccess), r - 1, i, usedSkills, true);
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