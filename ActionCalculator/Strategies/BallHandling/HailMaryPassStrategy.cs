using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class HailMaryPassStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var hailMaryPass = playerAction.Action;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var success = d6.Success(1, hailMaryPass.Roll);
            var failure = 1 - success;

            if (canUseSkill(CalculatorSkills.BlastIt, usedSkills))
            {
                usedSkills |= CalculatorSkills.BlastIt;
            }

            calculator.Resolve(p * success, r, i, usedSkills, true);

            if (proHelper.UsePro(player, hailMaryPass, r, usedSkills, success, success))
            {
                calculator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | CalculatorSkills.Pro, true);
                return;
            }
            
            calculator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills, true);
        }
    }
}