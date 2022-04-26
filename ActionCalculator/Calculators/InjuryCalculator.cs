using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class InjuryCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly ITwoD6 _twoD6;

        public InjuryCalculator(ICalculator calculator, ITwoD6 twoD6)
        {
            _calculator = calculator;
            _twoD6 = twoD6;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var success = action.Success;

            if (player.HasSkill(Skills.DirtyPlayer) && !usedSkills.HasFlag(Skills.DirtyPlayer))
            {
                success = _twoD6.Success(action.OriginalRoll - player.DirtyPlayerValue);
            }
            else if (player.HasSkill(Skills.MightyBlow) && !usedSkills.HasFlag(Skills.MightyBlow))
            {
                success = _twoD6.Success(action.OriginalRoll - player.MightyBlowValue);
            }
            else if (player.HasSkill(Skills.Ram) && !usedSkills.HasFlag(Skills.Ram))
            {
                success = _twoD6.Success(action.OriginalRoll - 1);
            }
            else if (player.HasSkill(Skills.Slayer) && !usedSkills.HasFlag(Skills.Slayer))
            {
                success = _twoD6.Success(action.OriginalRoll - 1);
            }

            _calculator.Calculate(p * success, r, playerAction, usedSkills, nonCriticalFailure);
        }
    }
}