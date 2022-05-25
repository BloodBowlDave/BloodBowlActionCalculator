using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Movement
{
    public class DodgeStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public DodgeStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var dodge = (Dodge) playerAction.Action;
            var success = dodge.Success;
            var failure = dodge.Failure;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var i = playerAction.Index;

            var useDivingTackle = dodge.UseDivingTackle && !usedSkills.Contains(Skills.DivingTackle);
            var useBreakTackleBeforeReroll = UseBreakTackleBeforeReroll(canUseSkill, dodge, r, usedSkills);
            var successIncludingBreakTackle = SuccessAfterModifiers(dodge, useDivingTackle, useBreakTackleBeforeReroll ? player.BreakTackleValue : 0);
            
            var failureWithDivingTackle = 0m;
            if (useDivingTackle)
            {
                failureWithDivingTackle = success - successIncludingBreakTackle;
                success -= failureWithDivingTackle;
            }

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            var successUsingBreakTackle = successIncludingBreakTackle - success;
            failure -= successUsingBreakTackle;

            if (useBreakTackleBeforeReroll)
            {
                _actionMediator.Resolve(p * successUsingBreakTackle, r, i, usedSkills | Skills.BreakTackle);
            }
            else if (CanUseBreakTackle(canUseSkill, usedSkills))
            {
                successIncludingBreakTackle = SuccessAfterModifiers(dodge, useDivingTackle, player.BreakTackleValue);
                successUsingBreakTackle = successIncludingBreakTackle - success;
            }

            if (canUseSkill(Skills.Dodge, usedSkills))
            {
                DodgeReroll(p, r, i, usedSkills | Skills.Dodge, failure, success, successUsingBreakTackle, failureWithDivingTackle);
                return;
            }

            if (_proHelper.UsePro(player, dodge, r, usedSkills, success, success))
            {
                DodgeReroll(p * proSuccess, r, i, usedSkills | Skills.Pro, failure, success, successUsingBreakTackle, failureWithDivingTackle);
                return;
            }
            
            DodgeReroll(p * lonerSuccess, r - 1, i, usedSkills, failure, success, successUsingBreakTackle, failureWithDivingTackle);
        }

        private static bool UseBreakTackleBeforeReroll(Func<Skills, Skills, bool> canUseSkill, Dodge dodge, int r, Skills usedSkills) =>
            CanUseBreakTackle(canUseSkill, usedSkills) && (dodge.UseBreakTackle || !PlayerCanRerollDodge(canUseSkill, usedSkills, r));

        private static bool PlayerCanRerollDodge(Func<Skills, Skills, bool> canUseSkill, Skills usedSkills, int r) =>
            canUseSkill(Skills.Dodge, usedSkills)
                || canUseSkill(Skills.Pro, usedSkills)
                || r > 0;
        
        private static bool CanUseBreakTackle(Func<Skills, Skills, bool> canUseSkill, Skills usedSkills) =>
            !usedSkills.Contains(Skills.BreakTackle) && (canUseSkill(Skills.BreakTackle, usedSkills) || canUseSkill(Skills.Incorporeal, usedSkills));

        private static decimal SuccessAfterModifiers(Dodge dodge, bool useDivingTackle, int breakTackleValue) =>
            (7m - (dodge.Roll + (useDivingTackle ? 2 : 0) - breakTackleValue).ThisOrMinimum(2).ThisOrMaximum(6)) / 6;

        private void DodgeReroll(decimal p, int r, int i, Skills usedSkills,
            decimal failure, decimal success, decimal useBreakTackle, decimal useDivingTackle)
        {
            DodgeReroll(p, r, i, usedSkills, failure, success, useDivingTackle);
            DodgeReroll(p, r, i, usedSkills | Skills.BreakTackle, failure, useBreakTackle, useDivingTackle);
        }

        private void DodgeReroll(decimal p, int r, int i, Skills usedSkills, decimal failure, decimal success, decimal useDivingTackle)
        {
            _actionMediator.Resolve(p * failure * success, r, i, usedSkills);
            _actionMediator.Resolve(p * useDivingTackle * success, r, i, usedSkills | Skills.DivingTackle);
        }
    }
}