using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.Movement
{
    public class RushStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var rush = playerAction.Action;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var success = d6.Success(1, rush.Roll);
            var failure = 1 - success;

            calculator.Resolve(p * success, r, i, usedSkills);

            p *= failure * success;

            if (canUseSkill(CalculatorSkills.SureFeet, usedSkills))
            {
                calculator.Resolve(p, r, i, usedSkills | CalculatorSkills.SureFeet);
                return;
            }

            if (proHelper.UsePro(player, rush, r, usedSkills, success, success))
            {
                calculator.Resolve(p * proSuccess, r, i, usedSkills | CalculatorSkills.Pro);
                return;
            }
        
            calculator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}