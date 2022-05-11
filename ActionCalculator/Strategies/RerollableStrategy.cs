using ActionCalculator.Abstractions;
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
            var ((rerollSuccess, proSuccess, canUseSkill), (success, failure), i) = playerAction;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure * success;

            if (_proHelper.CanUsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _actionMediator.Resolve(p * rerollSuccess, r - 1, i, usedSkills);
        }
    }
}