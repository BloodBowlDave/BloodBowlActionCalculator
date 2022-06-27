using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _d6;

        public FoulStrategy(IActionMediator actionMediator, ID6 d6)
        {
            _actionMediator = actionMediator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var canUseSkill = player.CanUseSkill;
            var roll = playerAction.Action.Roll;
            var success = _d6.Success(2, roll);
            var i = playerAction.Index;

            var canUseSneakyGit = canUseSkill(Skills.SneakyGit, usedSkills);
            var rollDouble = canUseSneakyGit ? 0 : _d6.RollDouble(roll, 12);
            var sentOffWithoutBreakingArmour = canUseSneakyGit ? 0 : _d6.RollDouble(2, roll - 1);

            _actionMediator.SendOff(p * sentOffWithoutBreakingArmour, r, i);
            
            _actionMediator.Resolve(p * (success - rollDouble), r, i, usedSkills);
            _actionMediator.Resolve(p * rollDouble, r, i, usedSkills, true);
            
            if (!canUseSkill(Skills.DirtyPlayer, usedSkills))
            {
	            return;
            }

            success = _d6.Success(2, roll - player.DirtyPlayerValue) - success;

            var rollDoubleAndUseDirtyPlayer = canUseSneakyGit ? 0 : _d6.RollDouble(roll - player.DirtyPlayerValue, roll - 1);

            _actionMediator.Resolve(p * (success - rollDoubleAndUseDirtyPlayer), r, i, usedSkills | Skills.DirtyPlayer);
            _actionMediator.Resolve(p * rollDoubleAndUseDirtyPlayer, r, i, usedSkills | Skills.DirtyPlayer, true);
        }
    }
}