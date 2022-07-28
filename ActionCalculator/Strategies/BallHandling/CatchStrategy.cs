﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class CatchStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public CatchStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var action = playerAction.Action;
            var success = _d6.Success(1, action.Numerator);
            var failure = 1 - success;

            _calculator.Resolve(p * success, r, i, usedSkills);

            p *= success * failure;

            if (canUseSkill(Skills.Catch, usedSkills))
            {
                _calculator.Resolve(p, r, i, usedSkills);
                return;
            }

            if (_proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                _calculator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _calculator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}