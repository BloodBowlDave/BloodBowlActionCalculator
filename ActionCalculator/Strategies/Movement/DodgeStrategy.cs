﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Strategies.Movement
{
    public class DodgeStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public DodgeStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var dodge = (Dodge) playerAction.Action;
            var success = _d6.Success(1, dodge.Roll);
            var failure = 1 - success;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var useDivingTackle = dodge.UseDivingTackle && !usedSkills.Contains(Skills.DivingTackle);
            var useBreakTackleBeforeReroll = UseBreakTackleBeforeReroll(canUseSkill, dodge, r, usedSkills);
            var successIncludingBreakTackle = SuccessAfterModifiers(dodge, useDivingTackle, useBreakTackleBeforeReroll ? player.BreakTackleValue : 0);
            
            var failureWithDivingTackle = 0m;
            if (useDivingTackle)
            {
                failureWithDivingTackle = success - successIncludingBreakTackle;
                success -= failureWithDivingTackle;
            }

            _calculator.Resolve(p * success, r, i, usedSkills);

            var successUsingBreakTackle = successIncludingBreakTackle - success;
            failure -= successUsingBreakTackle;

            if (useBreakTackleBeforeReroll)
            {
                _calculator.Resolve(p * successUsingBreakTackle, r, i, usedSkills | Skills.BreakTackle);
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

        private decimal SuccessAfterModifiers(Action dodge, bool useDivingTackle, int breakTackleValue)
        {
            var roll = dodge.Roll + (useDivingTackle ? 2 : 0) - breakTackleValue;
            return _d6.Success(1, roll);
        }

        private void DodgeReroll(decimal p, int r, int i, Skills usedSkills,
            decimal failure, decimal success, decimal useBreakTackle, decimal useDivingTackle)
        {
            DodgeReroll(p, r, i, usedSkills, failure, success, useDivingTackle);
            DodgeReroll(p, r, i, usedSkills | Skills.BreakTackle, failure, useBreakTackle, useDivingTackle);
        }

        private void DodgeReroll(decimal p, int r, int i, Skills usedSkills, decimal failure, decimal success, decimal useDivingTackle)
        {
            _calculator.Resolve(p * failure * success, r, i, usedSkills);
            _calculator.Resolve(p * useDivingTackle * success, r, i, usedSkills | Skills.DivingTackle);
        }
    }
}