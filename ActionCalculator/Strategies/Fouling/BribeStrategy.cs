using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.Fouling
{
    public class BribeStrategy(ICalculator calculator, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var success = d6.Success(1, 2);
            var failure = 1 - success;

            if (nonCriticalFailure)
            {
                calculator.Resolve(p * success, r, i, usedSkills);
                calculator.Resolve(p * failure, r, i, usedSkills, true);
                return;
            }

            calculator.Resolve(p, r, i, usedSkills);
        }
    }
}