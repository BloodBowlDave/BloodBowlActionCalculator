using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Fouling
{
    public class ArmourBreakCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly ITwoD6 _twoD6;

        public ArmourBreakCalculator(ICalculator calculator, ITwoD6 twoD6)
        {
            _calculator = calculator;
            _twoD6 = twoD6;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var useDpOnArmour = 0m;

            var hasDirtyPlayer = player.HasSkill(Skills.DirtyPlayer);
            if (hasDirtyPlayer)
            {
                useDpOnArmour = _twoD6.Success(action.OriginalRoll - 1) - action.Success;
            }

            var doubleOnArmour = player.HasSkill(Skills.SneakyGit) ? 0 : _twoD6.RollDouble(action.OriginalRoll);
            var doubleOnInjury = _twoD6.RollDouble(2);
            var noDouble = (1 - doubleOnArmour) * (1 - doubleOnInjury);

            if (action.RequiresRemoval)
            {
                var injuryRoll = player.HasSkill(Skills.Stunty) ? 7 : 8;

                var successWithoutUsingDpOnArmour = action.Success * _twoD6.Success(injuryRoll - (hasDirtyPlayer ? 1 : 0));
                var successUsingDpOnArmour = useDpOnArmour * _twoD6.Success(injuryRoll);
                var successWithRemoval = successWithoutUsingDpOnArmour + successUsingDpOnArmour;

                _calculator.Calculate(p * successWithRemoval * noDouble, r, playerAction, usedSkills);
                _calculator.Calculate(p * successWithRemoval * (1 - noDouble), r, playerAction, usedSkills, true);
                return;
            }

            var success = action.Success + useDpOnArmour;

            _calculator.Calculate(p * success * noDouble, r, playerAction, usedSkills);
            _calculator.Calculate(p * success * (1 - noDouble), r, playerAction, usedSkills, true);
        }
    }
}