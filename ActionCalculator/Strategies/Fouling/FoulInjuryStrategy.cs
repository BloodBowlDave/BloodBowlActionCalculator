using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulInjuryStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _d6;

        public FoulInjuryStrategy(ICalculator calculator, ID6 d6)
        {
            _calculator = calculator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool doubleOnFoul = false)
        {
            var player = playerAction.Player;
            var action = (Injury)playerAction.Action;
            var modifier = player.CanUseSkill(Skills.DirtyPlayer, usedSkills) ? player.DirtyPlayerValue : 0;
            var success = _d6.Success(2, action.Roll - modifier);
            
            if (doubleOnFoul)
            {
                _calculator.Resolve(p * success, r, i, usedSkills, true);
                return;
            }

            var successAndDoubleOnInjury = _d6.RollDouble(action.Roll - modifier, 12);
            _calculator.Resolve(p * (success - successAndDoubleOnInjury), r, i, usedSkills);
            _calculator.Resolve(p * successAndDoubleOnInjury, r, i, usedSkills, true);
        }
    }
}
