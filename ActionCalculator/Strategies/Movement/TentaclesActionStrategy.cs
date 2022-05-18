using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.Movement
{
    public class TentaclesActionStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public TentaclesActionStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((lonerSuccess, proSuccess, _), action, i) = playerAction;
            var (success, failure) = action;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure;

            if (_proHelper.UsePro(playerAction, r, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess, success, failure);
                return;
            }

            if (r > 0 && action.RerollNonCriticalFailure)
            {
                ExecuteReroll(p, r - 1, i, usedSkills | Skills.Pro, lonerSuccess, success, failure);
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal rerollSuccess, decimal success, decimal failure)
        {
            _actionMediator.Resolve(p * rerollSuccess * success, r, i, usedSkills);
            _actionMediator.Resolve(p * (1 - rerollSuccess + rerollSuccess * failure), r, i, usedSkills, true);
        }
    }
}