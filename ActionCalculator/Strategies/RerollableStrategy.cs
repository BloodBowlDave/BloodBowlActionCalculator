using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
    public class RerollableStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var (lonerSuccess, proSuccess, _) = player;

            var success = d6.Success(1, action.Roll);
            var failure = 1 - success;

            calculator.Resolve(p * success, r, i, usedSkills);

            p *= failure * success;

            if (proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                calculator.Resolve(p * proSuccess, r, i, usedSkills | CalculatorSkills.Pro);
                return;
            }
        
            calculator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}