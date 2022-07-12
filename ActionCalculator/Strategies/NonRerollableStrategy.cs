using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class NonRerollableStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _d6;

        public NonRerollableStrategy(IActionMediator actionMediator, ID6 d6)
        {
            _actionMediator = actionMediator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = (NonRerollable) playerAction.Action;
            var roll = action.Numerator;
            var denominator = action.Denominator;

            var success = denominator == 12 ? _d6.Success(2, roll) : (decimal)roll / denominator;
            
            var failure = 1 - success;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * success, r, i, usedSkills, nonCriticalFailure);

            if (player.CanUseSkill(Skills.WhirlingDervish, usedSkills) && !usedSkills.Contains(Skills.WhirlingDervish) && denominator == 6)
            {
                _actionMediator.Resolve(p * failure * success, r, i, usedSkills | Skills.WhirlingDervish);
            }
        }
    }
}