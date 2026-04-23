using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulInjuryStrategy(ICalculator calculator, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool doubleOnFoul = false)
        {
            var player = playerAction.Player;
            var action = (Injury)playerAction.Action;
            var modifier = player.CanUseSkill(CalculatorSkills.DirtyPlayer, usedSkills) ? player.DirtyPlayerValue : 0;
            var success = d6.Success(2, action.Roll - modifier);
            
            if (doubleOnFoul)
            {
                calculator.Resolve(p * success, r, i, usedSkills, true);
                return;
            }

            var successAndDoubleOnInjury = d6.RollDouble(action.Roll - modifier, 12);
            calculator.Resolve(p * (success - successAndDoubleOnInjury), r, i, usedSkills);
            calculator.Resolve(p * successAndDoubleOnInjury, r, i, usedSkills, true);
        }
    }
}
