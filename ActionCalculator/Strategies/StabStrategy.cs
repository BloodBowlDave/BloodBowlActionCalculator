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

            calculator.Resolve(p * success, r, i, usedSkills);
            calculator.Resolve(p * (1 - success), r, i, usedSkills, true);
        }
    }
}
