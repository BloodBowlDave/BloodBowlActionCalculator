using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class LandingStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProCalculator _proCalculator;

        public LandingStrategy(IActionMediator actionMediator, IProCalculator proCalculator)
        {
            _actionMediator = actionMediator;
            _proCalculator = proCalculator;
        }
        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, hasSkill), action, i) = playerAction;
            var success = nonCriticalFailure ? (7m - (action.OriginalRoll + 1).ThisOrMinimum(2).ThisOrMaximum(6)) / 6 : action.Success;
            var failure = 1m - success;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
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
