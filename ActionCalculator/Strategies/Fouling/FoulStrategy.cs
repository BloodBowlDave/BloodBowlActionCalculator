using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _d6;

        public FoulStrategy(ICalculator calculator, ID6 d6)
        {
            _calculator = calculator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var canUseSkill = player.CanUseSkill;
            var roll = playerAction.Action.Numerator;
            var success = _d6.Success(2, roll);

            var canUseSneakyGit = canUseSkill(Skills.SneakyGit, usedSkills);
            var rollDouble = canUseSneakyGit ? 0 : _d6.RollDouble(roll, 12);
            
            _calculator.Resolve(p * (success - rollDouble), r, i, usedSkills);
            _calculator.Resolve(p * rollDouble, r, i, usedSkills, true);
            
            if (!canUseSkill(Skills.DirtyPlayer, usedSkills))
            {
	            return;
            }

            success = _d6.Success(2, roll - player.DirtyPlayerValue) - success;

            var rollDoubleAndUseDirtyPlayer = canUseSneakyGit ? 0 : _d6.RollDouble(roll - player.DirtyPlayerValue, roll - 1);

            _calculator.Resolve(p * (success - rollDoubleAndUseDirtyPlayer), r, i, usedSkills | Skills.DirtyPlayer);
            _calculator.Resolve(p * rollDoubleAndUseDirtyPlayer, r, i, usedSkills | Skills.DirtyPlayer, true);
        }
    }
}