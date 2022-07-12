﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.BallHandling
{
    public class ThrowTeammateStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public ThrowTeammateStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var throwTeamMate = (ThrowTeammate) playerAction.Action;

            var modifier = throwTeamMate.Modifier;
            var modifiedRoll = throwTeamMate.Numerator - modifier;

            var successes = (7m - modifiedRoll).ThisOrMinimum(1).ThisOrMaximum(5);
            var failures = (1m - modifier).ThisOrMinimum(1).ThisOrMaximum(5);
            var inaccurateThrows = 6 - successes - failures;
            var success = successes / 6;
            var failure = failures / 6;

            var i = playerAction.Index;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            var inaccurateThrow = inaccurateThrows / 6;
            var rerollInaccurateThrow = throwTeamMate.RerollInaccurateThrow;
            var accurateThrowAfterFailure = (failure + (rerollInaccurateThrow ? inaccurateThrow : 0)) * success;
            var inaccurateThrowAfterFailure = (failure + (rerollInaccurateThrow ? inaccurateThrow : 0)) * inaccurateThrow;
            var inaccurateThrowWithoutReroll = rerollInaccurateThrow ? 0m : inaccurateThrow;

            if (canUseSkill(Skills.TheBallista, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills, accurateThrowAfterFailure, inaccurateThrowWithoutReroll + inaccurateThrowAfterFailure);
                return;
            }

            if (_proHelper.UsePro(player, throwTeamMate, r, usedSkills, success, success))
            {
                _actionMediator.Resolve(inaccurateThrowWithoutReroll, r, i, usedSkills, true);

                usedSkills |= Skills.Pro;
                ExecuteReroll(p * proSuccess, r, i, usedSkills | Skills.Pro, accurateThrowAfterFailure, inaccurateThrowAfterFailure);
                return;
            }

            if (r > 0 && rerollInaccurateThrow)
            {
                ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills | Skills.Pro, accurateThrowAfterFailure, inaccurateThrowAfterFailure);
                _actionMediator.Resolve(p * inaccurateThrow * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                return;
            }

            _actionMediator.Resolve(p * inaccurateThrow, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal accurateThrow, decimal inaccurateThrow)
        {
            _actionMediator.Resolve(p * accurateThrow, r, i, usedSkills);
            _actionMediator.Resolve(p * inaccurateThrow, r, i, usedSkills, true);
        }
    }
}