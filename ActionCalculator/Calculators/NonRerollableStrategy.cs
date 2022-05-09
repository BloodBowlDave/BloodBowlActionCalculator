using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Calculators
{
    public class NonRerollableStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;

        public NonRerollableStrategy(IActionMediator actionMediator)
        {
            _actionMediator = actionMediator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (player, action, i) = playerAction;
            var (success, failure) = action;

            _actionMediator.Resolve(p * action.Success, r, i, usedSkills, nonCriticalFailure);

            if (player.CanUseSkill(Skills.WhirlingDervish, usedSkills) && !usedSkills.Contains(Skills.WhirlingDervish) && action.Modifier == 6)
            {
                _actionMediator.Resolve(p * failure * success, r, i, usedSkills | Skills.WhirlingDervish);
            }
        }
    }
}