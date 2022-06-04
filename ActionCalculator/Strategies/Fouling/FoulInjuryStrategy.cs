using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies
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

            var doubleOnInjury = _d6.RollDouble(2);
            _actionMediator.Resolve(p * success * (1 - doubleOnInjury), r, i, usedSkills);
            _actionMediator.Resolve(p * success * doubleOnInjury, r, i, usedSkills, true);
            _actionMediator.SendOff(p * doubleOnInjury, r, i);
        }
    }
}
