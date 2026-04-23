using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class NonRerollableStrategy(ICalculator calculator, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = (NonRerollable) playerAction.Action;
            var roll = action.Roll;
            var denominator = action.Denominator;

            var success = denominator == 12 ? d6.Success(2, roll) : (decimal)roll / denominator;

            var failure = 1 - success;

            calculator.Resolve(p * success, r, i, usedSkills, nonCriticalFailure);

            if (player.CanUseSkill(CalculatorSkills.WhirlingDervish, usedSkills) && !usedSkills.Contains(CalculatorSkills.WhirlingDervish) && denominator == 6)
            {
                calculator.Resolve(p * failure * success, r, i, usedSkills | CalculatorSkills.WhirlingDervish);
            }
        }
    }
}