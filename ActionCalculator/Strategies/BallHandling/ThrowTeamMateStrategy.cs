using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.BallHandling
{
    public class ThrowTeammateStrategy(ICalculator calculator, IProHelper proHelper) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var throwTeamMate = (ThrowTeammate) playerAction.Action;

            var modifier = throwTeamMate.Modifier;
            var modifiedRoll = throwTeamMate.Roll - modifier;

            var successes = (7m - modifiedRoll).ThisOrMinimum(1).ThisOrMaximum(5);
            var failures = (1m - modifier).ThisOrMinimum(1).ThisOrMaximum(5);
            var inaccurateThrows = 6 - successes - failures;
            var success = successes / 6;
            var failure = failures / 6;

            calculator.Resolve(p * success, r, i, usedSkills);

            var inaccurateThrow = inaccurateThrows / 6;
            var rerollInaccurateThrow = throwTeamMate.RerollInaccurateThrow;
            var accurateThrowAfterFailure = (failure + (rerollInaccurateThrow ? inaccurateThrow : 0)) * success;
            var inaccurateThrowAfterFailure = (failure + (rerollInaccurateThrow ? inaccurateThrow : 0)) * inaccurateThrow;
            var inaccurateThrowWithoutReroll = rerollInaccurateThrow ? 0m : inaccurateThrow;

            if (canUseSkill(CalculatorSkills.TheBallista, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills, accurateThrowAfterFailure, inaccurateThrowWithoutReroll + inaccurateThrowAfterFailure);
                return;
            }

            if (proHelper.UsePro(player, throwTeamMate, r, usedSkills, success, success))
            {
                calculator.Resolve(p * inaccurateThrowWithoutReroll, r, i, usedSkills, true);

                usedSkills |= CalculatorSkills.Pro;
                ExecuteReroll(p * proSuccess, r, i, usedSkills | CalculatorSkills.Pro, accurateThrowAfterFailure, inaccurateThrowAfterFailure);
                return;
            }

            if (r > 0 && rerollInaccurateThrow)
            {
                ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills | CalculatorSkills.Pro, accurateThrowAfterFailure, inaccurateThrowAfterFailure);
                calculator.Resolve(p * inaccurateThrow * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                return;
            }

            calculator.Resolve(p * inaccurateThrow, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, CalculatorSkills usedSkills, decimal accurateThrow, decimal inaccurateThrow)
        {
            calculator.Resolve(p * accurateThrow, r, i, usedSkills);
            calculator.Resolve(p * inaccurateThrow, r, i, usedSkills, true);
        }
    }
}