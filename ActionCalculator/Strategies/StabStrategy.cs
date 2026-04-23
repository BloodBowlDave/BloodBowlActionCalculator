using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
    public class StabStrategy(ICalculator calculator, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var modifier = player.CanUseSkill(CalculatorSkills.ASneakyPair, usedSkills) ? 1 : 0;
            var success = d6.Success(2, action.Roll - modifier);
            var failure = 1m - success;

            calculator.Resolve(p * success, r, i, usedSkills);

            if (player.CanUseSkill(CalculatorSkills.MasterAssassin, usedSkills)) 
            {
                calculator.Resolve(p * failure * success, r, i, usedSkills | CalculatorSkills.MasterAssassin);
                calculator.Resolve(p * failure * failure, r, i, usedSkills | CalculatorSkills.MasterAssassin, true);
                return;
            }

            calculator.Resolve(p * failure, r, i, usedSkills, true);
        }
    }
}
