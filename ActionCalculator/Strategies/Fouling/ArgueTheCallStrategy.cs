using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Fouling
{
    public class ArgueTheCallStrategy(ICalculator calculator, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            if (nonCriticalFailure)
            {
                var argueTheCall = (ArgueTheCall)playerAction.Action;
                var success = d6.Success(1, argueTheCall.Roll);
                var failure = 1 - success - 1m / 6;

                calculator.Resolve(p * success, r, i, usedSkills);
                calculator.Resolve(p * failure, r, i, usedSkills, true);
                return;
            }

            calculator.Resolve(p, r, i, usedSkills);
        }
    }
}