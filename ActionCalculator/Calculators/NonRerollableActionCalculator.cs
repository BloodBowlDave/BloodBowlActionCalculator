using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Calculators
{
    public class NonRerollableActionCalculator : ICalculator
    {
        private readonly ICalculator _calculator;

        public NonRerollableActionCalculator(ICalculator calculator)
        {
            _calculator = calculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;

            _calculator.Calculate(p * action.Success, r, playerAction, usedSkills, nonCriticalFailure);

            if (player.HasSkill(Skills.WhirlingDervish) && !usedSkills.Contains(Skills.WhirlingDervish) && action.Modifier == 6)
            {
                _calculator.Calculate(p * action.Failure * action.Success, r, playerAction, usedSkills | Skills.WhirlingDervish);
            }
        }
    }
}