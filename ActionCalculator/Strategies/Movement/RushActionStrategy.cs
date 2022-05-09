using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.Movement
{
    public class RushActionStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public RushActionStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, canUseSkill), (success, failure), i) = playerAction;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure * success;

            if (canUseSkill(Skills.SureFeet, usedSkills))
            {
                _actionMediator.Resolve(p, r, i, usedSkills | Skills.SureFeet);
                return;
            }

            if (_proHelper.CanUsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _actionMediator.Resolve(p * rerollSuccess, r - 1, i, usedSkills);
            }
        }
    }
}