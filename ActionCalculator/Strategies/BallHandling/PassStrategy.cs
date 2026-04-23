using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.BallHandling
{
    public class PassStrategy(ICalculator calculator, IProHelper proHelper, ICalculationContext context) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var pass = (Pass) playerAction.Action;

            var modifier = pass.Modifier;
            var modifiedRoll = pass.Roll - modifier;

            var successes = (7m - modifiedRoll).ThisOrMinimum(1).ThisOrMaximum(5);
            var failures = (1m - modifier).ThisOrMinimum(1).ThisOrMaximum(5);
            var inaccuratePasses = 6m - successes - failures;

            var success = successes / 6;
            var failure = failures / 6;

            calculator.Resolve(p * success, r, i, usedSkills);

            var inaccuratePass = inaccuratePasses / 6;
            var rerollInaccuratePass = pass.RerollInaccuratePass;
            var accuratePassAfterFailure = (failure + (rerollInaccuratePass ? inaccuratePass : 0)) * success;
            var inaccuratePassAfterFailure = (failure + (rerollInaccuratePass ? inaccuratePass : 0)) * inaccuratePass;
            var inaccuratePassWithoutReroll = rerollInaccuratePass ? 0m : inaccuratePass;

            if (context.Season != Season.Season3 && canUseSkill(CalculatorSkills.TheBallista, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills, accuratePassAfterFailure, inaccuratePassWithoutReroll + inaccuratePassAfterFailure);
                return;
            }

            if (canUseSkill(CalculatorSkills.Pass, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills, accuratePassAfterFailure, inaccuratePassWithoutReroll + inaccuratePassAfterFailure);
                return;
            }

            if (proHelper.UsePro(player, pass, r, usedSkills, success, success))
            {
                calculator.Resolve(p * inaccuratePassWithoutReroll, r, i, usedSkills, true);

                usedSkills |= CalculatorSkills.Pro;
                ExecuteReroll(p * proSuccess, r, i, usedSkills | CalculatorSkills.Pro, accuratePassAfterFailure, inaccuratePassAfterFailure);
                return;
            }

            if (r > 0 && rerollInaccuratePass)
            {
                ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills | CalculatorSkills.Pro, accuratePassAfterFailure, inaccuratePassAfterFailure);
                calculator.Resolve(p * inaccuratePass * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                return;
            }

            calculator.Resolve(p * inaccuratePass, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, CalculatorSkills usedSkills, decimal accuratePass, decimal inaccuratePass)
        {
            calculator.Resolve(p * accuratePass, r, i, usedSkills);
            calculator.Resolve(p * inaccuratePass, r, i, usedSkills, true);
        }
    }
}