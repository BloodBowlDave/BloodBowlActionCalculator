﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Blocking
{
    public class DauntlessStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public DauntlessStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var dauntless = (Dauntless) playerAction.Action;

            var success = _d6.Success(1, dauntless.Roll);
            var failure = 1 - success;

            _calculator.Resolve(p * success, r, i, usedSkills);
            
            p *= failure;

            if (dauntless.RerollFailure)
            {
                if (canUseSkill(Skills.BlindRage, usedSkills))
                {
                    ExecuteReroll(p, r, i, usedSkills | Skills.BlindRage, success, failure);
                    return;
                }

                if (_proHelper.UsePro(player, dauntless, r, usedSkills, success, success))
                {
                    ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess * success, proSuccess * failure + (1 - proSuccess));
                    return;
                }

                if (r > 0)
                {
                    _calculator.Resolve(p * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                    ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills, success, failure);
                    return;
                }
            }

            if (_proHelper.UsePro(player, dauntless, r, usedSkills, success, success))
            {
                ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess * success, proSuccess * failure + (1 - proSuccess));
                return;
            }

            _calculator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal success, decimal failure)
        {
            _calculator.Resolve(p * success, r, i, usedSkills);
            _calculator.Resolve(p * failure, r, i, usedSkills, true);
        }
    }
}