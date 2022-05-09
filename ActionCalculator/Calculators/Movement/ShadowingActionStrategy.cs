﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Movement
{
    public class ShadowingActionStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProCalculator _proCalculator;

        public ShadowingActionStrategy(IActionMediator actionMediator, IProCalculator proCalculator)
        {
            _actionMediator = actionMediator;
            _proCalculator = proCalculator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, _), action, i) = playerAction;
            var (success, failure) = action;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= action.Failure;

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess, success, failure);
                return;
            }

            if (r > 0 && action.RerollNonCriticalFailure)
            {
                ExecuteReroll(p, r - 1, i, usedSkills, rerollSuccess, success, failure);
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal rerollSuccess, decimal success, decimal failure)
        {
            _actionMediator.Resolve(p * rerollSuccess * success, r, i, usedSkills);
            _actionMediator.Resolve(p * (1 - rerollSuccess + rerollSuccess * failure), r, i, usedSkills, true);
        }
    }
}