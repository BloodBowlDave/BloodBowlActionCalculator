using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class HypnogazeCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public HypnogazeCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;

            _calculator.Calculate(p * action.Success, r, playerAction, usedSkills);

            p *= action.Failure;

            if (player.HasSkill(Skills.MesmerisingDance))
            {
                _calculator.Calculate(p * action.Success, r, playerAction, usedSkills);
                _calculator.Calculate(p * action.Failure, r, playerAction, usedSkills, true);
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                p *= player.ProSuccess;
                usedSkills |= Skills.Pro;
                _calculator.Calculate(p * action.Success, r, playerAction, usedSkills);
                _calculator.Calculate(p * action.Failure, r, playerAction, usedSkills, true);
                return;
            }

            if (r > 0 && action.RerollNonCriticalFailure)
            {
                _calculator.Calculate(p * action.Success, r - 1, playerAction, usedSkills);
                _calculator.Calculate(p * action.Failure, r - 1, playerAction, usedSkills, true);
                return;
            }

            _calculator.Calculate(p, r, playerAction, usedSkills, true);
        }
    }
}