using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
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
            var player = playerAction.Player;
            var action = (NonRerollableAction) playerAction.Action;
            var success = action.Success;
            var failure = action.Failure;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * action.Success, r, i, usedSkills, nonCriticalFailure);

            if (player.CanUseSkill(Skills.WhirlingDervish, usedSkills) && !usedSkills.Contains(Skills.WhirlingDervish) && action.Denominator == 6)
            {
                _actionMediator.Resolve(p * failure * success, r, i, usedSkills | Skills.WhirlingDervish);
            }
        }
    }
}