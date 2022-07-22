﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class PickUpStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public PickUpStrategy(IActionMediator actionMediator, IProHelper proHelper, ID6 d6)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var pickUp = playerAction.Action;

            var success = _d6.Success(1, pickUp.Numerator);
            var failure = 1 - success;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure * success;

            if (canUseSkill(Skills.SureHands, usedSkills))
            {
                _actionMediator.Resolve(p, r, i, usedSkills);
                return;
            }

            if (_proHelper.UsePro(player, pickUp, r, usedSkills, success, success))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _actionMediator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}