using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Blocking
{
    public class DauntlessStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var dauntless = (Dauntless) playerAction.Action;

            var success = d6.Success(1, dauntless.Roll);
            var failure = 1 - success;

            calculator.Resolve(p * success, r, i, usedSkills);

            p *= failure;

            if (dauntless.RerollFailure)
            {
                if (canUseSkill(CalculatorSkills.BlindRage, usedSkills))
                {
                    ExecuteReroll(p, r, i, usedSkills | CalculatorSkills.BlindRage, success, failure);
                    return;
                }

                if (proHelper.UsePro(player, dauntless, r, usedSkills, success, success))
                {
                    ExecuteReroll(p, r, i, usedSkills | CalculatorSkills.Pro, proSuccess * success, proSuccess * failure + (1 - proSuccess));
                    return;
                }

                if (r > 0)
                {
                    calculator.Resolve(p * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                    ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills, success, failure);
                    return;
                }
            }

            if (proHelper.UsePro(player, dauntless, r, usedSkills, success, success))
            {
                ExecuteReroll(p, r, i, usedSkills | CalculatorSkills.Pro, proSuccess * success, proSuccess * failure + (1 - proSuccess));
                return;
            }

            calculator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, CalculatorSkills usedSkills, decimal success, decimal failure)
        {
            calculator.Resolve(p * success, r, i, usedSkills);
            calculator.Resolve(p * failure, r, i, usedSkills, true);
        }
    }
}