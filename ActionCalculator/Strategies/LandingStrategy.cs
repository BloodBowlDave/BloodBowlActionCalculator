using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class LandingStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public LandingStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }
        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, canUseSkill), action, i) = playerAction;
            var success = nonCriticalFailure ? (7m - (action.OriginalRoll + 1).ThisOrMinimum(2).ThisOrMaximum(6)) / 6 : action.Success;
            var failure = 1m - success;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            if (_proHelper.CanUsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _actionMediator.Resolve(p * failure * rerollSuccess * success, r - 1, i, usedSkills);
            }
        }
    }
}
