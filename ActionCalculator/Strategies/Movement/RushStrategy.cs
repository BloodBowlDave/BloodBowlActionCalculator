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

            var pSteadyFooting = canUseSkill(CalculatorSkills.SteadyFooting, usedSkills) ? 1m / 6 : 0;
 
            if (canUseSkill(CalculatorSkills.SureFeet, usedSkills))
            {
                calculator.Resolve(p * failure * success, r, i, usedSkills | CalculatorSkills.SureFeet);
                calculator.Resolve(p * failure * failure * pSteadyFooting, r, i, usedSkills | CalculatorSkills.SureFeet);
                calculator.Resolve(p * failure * failure * (1m - pSteadyFooting) * lonerSuccess * pSteadyFooting, r - 1, i, usedSkills | CalculatorSkills.SureFeet);
                return;
            }

            if (proHelper.UsePro(player, rush, r, usedSkills, success, success))
            {
                calculator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | CalculatorSkills.Pro);
                calculator.Resolve(p * failure * proSuccess * failure * pSteadyFooting, r, i, usedSkills | CalculatorSkills.Pro);
                calculator.Resolve(p * failure * proSuccess * failure * (1m - pSteadyFooting) * lonerSuccess * pSteadyFooting, r - 1, i, usedSkills | CalculatorSkills.Pro);
                calculator.Resolve(p * failure * (1m - proSuccess) * pSteadyFooting, r, i, usedSkills | CalculatorSkills.Pro);
                calculator.Resolve(p * failure * (1m - proSuccess) * (1m - pSteadyFooting) * lonerSuccess * pSteadyFooting, r - 1, i, usedSkills | CalculatorSkills.Pro);
                return;
            }

            if (r == 0)
            {
                calculator.Resolve(p * failure * pSteadyFooting, r, i, usedSkills);
            }
            else
            {
                calculator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills);
                calculator.Resolve(p * failure * lonerSuccess * failure * pSteadyFooting, r - 1, i, usedSkills);
                calculator.Resolve(p * failure * lonerSuccess * failure * (1m - pSteadyFooting) * lonerSuccess * pSteadyFooting, r - 2, i, usedSkills);
                calculator.Resolve(p * failure * (1m - lonerSuccess) * pSteadyFooting, r - 1, i, usedSkills);
                calculator.Resolve(p * failure * (1m - lonerSuccess) * (1m - pSteadyFooting) * lonerSuccess * pSteadyFooting, r - 2, i, usedSkills);
            }
        }
    }
}