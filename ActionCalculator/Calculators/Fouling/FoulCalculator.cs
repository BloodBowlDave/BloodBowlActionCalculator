using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Fouling
{
    public class FoulCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly ITwoD6 _twoD6;

        public FoulCalculator(ICalculator calculator, ITwoD6 twoD6)
        {
            _calculator = calculator;
            _twoD6 = twoD6;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var useDpOnArmour = 0m;
            var success = action.Success;

            var hasDirtyPlayer = player.HasSkill(Skills.DirtyPlayer);
            if (hasDirtyPlayer)
            {
                useDpOnArmour = _twoD6.Success(action.OriginalRoll - player.DirtyPlayerValue) - action.Success;
            }

            var doubleOnArmour = player.HasSkill(Skills.SneakyGit) ? 0 : _twoD6.RollDouble(action.OriginalRoll);
            var doubleOnInjury = _twoD6.RollDouble(2);
            var noDouble = (1 - doubleOnArmour) * (1 - doubleOnInjury);
            
            Calculate(p * success * noDouble, p * success * (1 - noDouble), r, playerAction, usedSkills);
            p *= useDpOnArmour;
            Calculate(p * noDouble, p * (1 - noDouble), r, playerAction, usedSkills | Skills.DirtyPlayer);
        }

        private void Calculate(decimal successNoDouble, decimal successWithDouble, int r, PlayerAction playerAction, Skills usedSkills)
        {
            _calculator.Calculate(successNoDouble, r, playerAction, usedSkills);
            _calculator.Calculate(successWithDouble, r, playerAction, usedSkills, true);
        }
    }
}