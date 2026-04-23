using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
	public class ChainsawStrategy(ICalculator calculator, ID6 d6) : IActionStrategy
	{
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
		{
			var action = playerAction.Action;
			var success = d6.Success(2, action.Roll);

			calculator.Resolve(p * success, r, i, usedSkills);
			calculator.Resolve(p * (1 - success), r, i, usedSkills, true);
		}
	}
}