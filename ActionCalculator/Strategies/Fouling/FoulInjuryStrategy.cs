﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulInjuryStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _d6;

        public FoulInjuryStrategy(IActionMediator actionMediator, ID6 d6)
        {
            _actionMediator = actionMediator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool doubleOnFoul = false)
        {
            var player = playerAction.Player;
            var action = (Injury)playerAction.Action;
            var i = playerAction.Index;
            var modifier = player.CanUseSkill(Skills.DirtyPlayer, usedSkills) ? player.DirtyPlayerValue : 0;
            var success = _d6.Success(2, action.Roll - modifier);
            
            if (doubleOnFoul)
            {
                _actionMediator.Resolve(p * success, r, i, usedSkills, true);
                _actionMediator.SendOff(p, r, i);
                return;
            }

            var successAndDoubleOnInjury = _d6.RollDouble(action.Roll - modifier, 12);
            _actionMediator.Resolve(p * (success - successAndDoubleOnInjury), r, i, usedSkills);
            _actionMediator.Resolve(p * successAndDoubleOnInjury, r, i, usedSkills, true);
            _actionMediator.SendOff(p * _d6.RollDouble(2, 12), r, i);
        }
    }
}
