using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Movement
{
    public class ShadowingStrategy(ICalculator calculator, IProHelper proHelper) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, _) = player;
            var shadowing = (Shadowing) playerAction.Action;

            var failure = (decimal)(shadowing.Roll + 1).ThisOrMinimum(1).ThisOrMaximum(6) / 6;
            var success = 1 - failure;

            calculator.Resolve(p * success, r, i, usedSkills);

            p *= failure;

            if (proHelper.UsePro(player, shadowing, r, usedSkills, success, success))
            {
                ExecuteReroll(p, r, i, usedSkills | CalculatorSkills.Pro, proSuccess, success, failure);
                return;
            }

            if (r > 0 && shadowing.RerollFailure)
            {
                ExecuteReroll(p, r - 1, i, usedSkills, lonerSuccess, success, failure);
                return;
            }

            calculator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, CalculatorSkills usedSkills, decimal rerollSuccess, decimal success, decimal failure)
        {
            calculator.Resolve(p * rerollSuccess * success, r, i, usedSkills);
            calculator.Resolve(p * (1 - rerollSuccess + rerollSuccess * failure), r, i, usedSkills, true);
        }
    }
}