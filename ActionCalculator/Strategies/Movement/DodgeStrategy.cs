using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Strategies.Movement
{
    public class DodgeStrategy(ICalculator calculator, IProHelper proHelper, IConsummateProfessionalHelper consummateProfessionalHelper, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var dodge = (Dodge) playerAction.Action;
            var success = d6.Success(1, dodge.Roll);
            var failure = 1 - success;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var useDivingTackle = dodge.UseDivingTackle && !usedSkills.Contains(CalculatorSkills.DivingTackle);
            var useBreakTackleBeforeReroll = UseBreakTackleBeforeReroll(canUseSkill, dodge, r, usedSkills);
            var successIncludingBreakTackle = SuccessAfterModifiers(dodge, useDivingTackle, useBreakTackleBeforeReroll ? player.BreakTackleValue : 0);

            var failureWithDivingTackle = 0m;
            if (useDivingTackle)
            {
                failureWithDivingTackle = success - successIncludingBreakTackle;
                success -= failureWithDivingTackle;
            }

            calculator.Resolve(p * success, r, i, usedSkills);

            var breakTackleBeforeReroll = useBreakTackleBeforeReroll ? player.BreakTackleValue : 0;
            var modifiedRoll = dodge.Roll + (useDivingTackle ? 2 : 0) - breakTackleBeforeReroll;
            var consummateProfessionalSuccess = consummateProfessionalHelper.GetAgilityRollSuccess(usedSkills, modifiedRoll, canUseSkill);

            if (consummateProfessionalSuccess > 0)
            {
                var consummateProfessionalSkills = usedSkills | CalculatorSkills.Pro | (breakTackleBeforeReroll > 0 ? CalculatorSkills.BreakTackle : CalculatorSkills.None);
                calculator.Resolve(p * consummateProfessionalSuccess, r, i, consummateProfessionalSkills);
            }
            failure -= consummateProfessionalSuccess;

            var successUsingBreakTackle = successIncludingBreakTackle - success;
            failure -= successUsingBreakTackle;

            if (useBreakTackleBeforeReroll)
            {
                calculator.Resolve(p * successUsingBreakTackle, r, i, usedSkills | CalculatorSkills.BreakTackle);
            }
            else if (CanUseBreakTackle(canUseSkill, usedSkills))
            {
                successIncludingBreakTackle = SuccessAfterModifiers(dodge, useDivingTackle, player.BreakTackleValue);
                successUsingBreakTackle = successIncludingBreakTackle - success;
            }

            var pSteadyFooting = canUseSkill(CalculatorSkills.SteadyFooting, usedSkills) ? 1m / 6 : 0;

            if (canUseSkill(CalculatorSkills.Dodge, usedSkills))
            {
                DodgeReroll(p, r, i, usedSkills | CalculatorSkills.Dodge, failure, success, successUsingBreakTackle, failureWithDivingTackle, pSteadyFooting, lonerSuccess);
                return;
            }

            if (proHelper.UsePro(player, dodge, r, usedSkills, success, success))
            {
                DodgeReroll(p * proSuccess, r, i, usedSkills | CalculatorSkills.Pro, failure, success, successUsingBreakTackle, failureWithDivingTackle, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * (1m - proSuccess) * failure, r, i, usedSkills | CalculatorSkills.Pro, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * (1m - proSuccess) * failureWithDivingTackle, r, i, usedSkills | CalculatorSkills.Pro | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
                return;
            }

            if (r == 0)
            {
                ResolveSteadyFooting(p * failure, r, i, usedSkills, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * failureWithDivingTackle, r, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
            }
            else
            {
                DodgeReroll(p * lonerSuccess, r - 1, i, usedSkills, failure, success, successUsingBreakTackle, failureWithDivingTackle, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * (1m - lonerSuccess) * failure, r - 1, i, usedSkills, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * (1m - lonerSuccess) * failureWithDivingTackle, r - 1, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
            }
        }

        private void ResolveSteadyFooting(decimal p, int r, int i, CalculatorSkills usedSkills, decimal pSteadyFooting, decimal lonerSuccess)
        {
            if (pSteadyFooting == 0) return;
            calculator.Resolve(p * pSteadyFooting, r, i, usedSkills | CalculatorSkills.SteadyFooting);
            calculator.Resolve(p * (1m - pSteadyFooting) * lonerSuccess * pSteadyFooting, r - 1, i, usedSkills | CalculatorSkills.SteadyFooting);
        }

        private static bool UseBreakTackleBeforeReroll(Func<CalculatorSkills, CalculatorSkills, bool> canUseSkill, Dodge dodge, int r, CalculatorSkills usedSkills) =>
            CanUseBreakTackle(canUseSkill, usedSkills) && (dodge.UseBreakTackle || !PlayerCanRerollDodge(canUseSkill, usedSkills, r));

        private static bool PlayerCanRerollDodge(Func<CalculatorSkills, CalculatorSkills, bool> canUseSkill, CalculatorSkills usedSkills, int r) =>
            canUseSkill(CalculatorSkills.Dodge, usedSkills)
                || canUseSkill(CalculatorSkills.Pro, usedSkills)
                || r > 0;

        private static bool CanUseBreakTackle(Func<CalculatorSkills, CalculatorSkills, bool> canUseSkill, CalculatorSkills usedSkills) =>
            !usedSkills.Contains(CalculatorSkills.BreakTackle) && canUseSkill(CalculatorSkills.BreakTackle, usedSkills);

        private decimal SuccessAfterModifiers(Action dodge, bool useDivingTackle, int breakTackleValue)
        {
            var roll = dodge.Roll + (useDivingTackle ? 2 : 0) - breakTackleValue;
            return d6.Success(1, roll);
        }

        private void DodgeReroll(decimal p, int r, int i, CalculatorSkills usedSkills,
            decimal failure, decimal success, decimal useBreakTackle, decimal useDivingTackle,
            decimal pSteadyFooting, decimal lonerSuccess)
        {
            DodgeReroll(p, r, i, usedSkills, failure, success, useDivingTackle);
            DodgeReroll(p, r, i, usedSkills | CalculatorSkills.BreakTackle, failure, useBreakTackle, useDivingTackle);
            var terminalFailure = p * failure * (1m - success - useBreakTackle);
            ResolveSteadyFooting(terminalFailure, r, i, usedSkills, pSteadyFooting, lonerSuccess);
            var terminalFailureDt = p * useDivingTackle * (1m - success - useBreakTackle);
            ResolveSteadyFooting(terminalFailureDt, r, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
        }

        private void DodgeReroll(decimal p, int r, int i, CalculatorSkills usedSkills, decimal failure, decimal success, decimal useDivingTackle)
        {
            calculator.Resolve(p * failure * success, r, i, usedSkills);
            calculator.Resolve(p * useDivingTackle * success, r, i, usedSkills | CalculatorSkills.DivingTackle);
        }
    }
}
