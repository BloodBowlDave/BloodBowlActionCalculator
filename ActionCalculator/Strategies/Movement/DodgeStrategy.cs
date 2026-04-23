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

            var consummateProfessionalSuccess = consummateProfessionalHelper.GetAgilityRollSuccess(usedSkills, dodge.Roll, canUseSkill);

            if (consummateProfessionalSuccess > 0)
            {
                calculator.Resolve(p * consummateProfessionalSuccess, r, i, usedSkills | CalculatorSkills.Pro);
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

            if (canUseSkill(CalculatorSkills.Dodge, usedSkills))
            {
                DodgeReroll(p, r, i, usedSkills | CalculatorSkills.Dodge, failure, success, successUsingBreakTackle, failureWithDivingTackle);
                return;
            }

            if (proHelper.UsePro(player, dodge, r, usedSkills, success, success))
            {
                DodgeReroll(p * proSuccess, r, i, usedSkills | CalculatorSkills.Pro, failure, success, successUsingBreakTackle, failureWithDivingTackle);
                return;
            }
            
            DodgeReroll(p * lonerSuccess, r - 1, i, usedSkills, failure, success, successUsingBreakTackle, failureWithDivingTackle);
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
            decimal failure, decimal success, decimal useBreakTackle, decimal useDivingTackle)
        {
            DodgeReroll(p, r, i, usedSkills, failure, success, useDivingTackle);
            DodgeReroll(p, r, i, usedSkills | CalculatorSkills.BreakTackle, failure, useBreakTackle, useDivingTackle);
        }

        private void DodgeReroll(decimal p, int r, int i, CalculatorSkills usedSkills, decimal failure, decimal success, decimal useDivingTackle)
        {
            calculator.Resolve(p * failure * success, r, i, usedSkills);
            calculator.Resolve(p * useDivingTackle * success, r, i, usedSkills | CalculatorSkills.DivingTackle);
        }
    }
}