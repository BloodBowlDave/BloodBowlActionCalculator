using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

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
            _calculator.Calculate(p * playerAction.Action.Success, r, playerAction, usedSkills, nonCriticalFailure);
        }
    }
}