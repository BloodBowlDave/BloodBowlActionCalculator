using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.Calculators.Movement
{
    public class DodgeCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public DodgeCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var useDivingTackle = action.UseDivingTackle && !usedSkills.Contains(Skills.DivingTackle);
            var useBreakTackleBeforeReroll = UseBreakTackleBeforeReroll(player, action, r, usedSkills);
            var successIncludingBreakTackle = SuccessAfterModifiers(playerAction, useDivingTackle, useBreakTackleBeforeReroll ? player.BreakTackleValue : 0);
            var success = action.Success;
            
            var failureWithDivingTackle = 0m;
            if (useDivingTackle)
            {
                failureWithDivingTackle = success - successIncludingBreakTackle;
                success -= failureWithDivingTackle;
            }

            _calculator.Calculate(p * success, r, playerAction, usedSkills);

            var failure = action.Failure;
            var successUsingBreakTackle = successIncludingBreakTackle - success;
            failure -= successUsingBreakTackle;

            if (useBreakTackleBeforeReroll)
            {
                _calculator.Calculate(p * successUsingBreakTackle, r, playerAction, usedSkills | Skills.BreakTackle);
            }
            else if (CanUseBreakTackle(player, usedSkills))
            {
                successIncludingBreakTackle = SuccessAfterModifiers(playerAction, useDivingTackle, player.BreakTackleValue);
                successUsingBreakTackle = successIncludingBreakTackle - success;
            }

            if (player.HasSkill(Skills.Dodge) && !usedSkills.Contains(Skills.Dodge))
            {
                CalculateDodgeReroll(p, r, playerAction, usedSkills | Skills.Dodge, failure, success, successUsingBreakTackle, failureWithDivingTackle);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                CalculateDodgeReroll(p * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro, failure, success, successUsingBreakTackle, failureWithDivingTackle);
                return;
            }

            if (r == 0)
            {
                return;
            }

            CalculateDodgeReroll(p * player.UseReroll, r - 1, playerAction, usedSkills, failure, success, successUsingBreakTackle, failureWithDivingTackle);
        }

        private static bool UseBreakTackleBeforeReroll(Player player, Action action, int r, Skills usedSkills) =>
            CanUseBreakTackle(player, usedSkills) && (action.UseBreakTackle || !PlayerCanRerollDodge(player, usedSkills, r));

        private static bool PlayerCanRerollDodge(Player player, Skills usedSkills, int r) =>
            player.HasSkill(Skills.Dodge) && !usedSkills.Contains(Skills.Dodge)
                || player.HasSkill(Skills.Pro) && !usedSkills.Contains(Skills.Pro)
                || r > 0;
        
        private static bool CanUseBreakTackle(Player player, Skills usedSkills) =>
            !usedSkills.Contains(Skills.BreakTackle) && (player.HasSkill(Skills.BreakTackle) || player.HasSkill(Skills.Incorporeal));

        private static decimal SuccessAfterModifiers(PlayerAction playerAction, bool useDivingTackle, int breakTackleValue) =>
            (7m - (playerAction.Action.OriginalRoll + (useDivingTackle ? 2 : 0) - breakTackleValue).ThisOrMinimum(2).ThisOrMaximum(6)) / 6;

        private void CalculateDodgeReroll(decimal p, int r, PlayerAction playerAction, Skills usedSkills,
            decimal failure, decimal success, decimal useBreakTackle, decimal useDivingTackle)
        {
            _calculator.Calculate(p * failure * success, r, playerAction, usedSkills);
            _calculator.Calculate(p * useDivingTackle * success, r, playerAction, usedSkills | Skills.DivingTackle);
            usedSkills |= Skills.BreakTackle;
            _calculator.Calculate(p * failure * useBreakTackle, r, playerAction, usedSkills);
            _calculator.Calculate(p * useDivingTackle * useBreakTackle, r, playerAction, usedSkills | Skills.DivingTackle);
        }
    }
}