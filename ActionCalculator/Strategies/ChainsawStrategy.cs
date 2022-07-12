using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
	public class ChainsawStrategy : IActionStrategy
	{
		private readonly IActionMediator _actionMediator;
		private readonly ID6 _d6;

		public ChainsawStrategy(IActionMediator actionMediator, ID6 d6)
		{
			_actionMediator = actionMediator;
			_d6 = d6;
		}

		public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
		{
			var action = playerAction.Action;
			var success = _d6.Success(2, action.Numerator);
			var i = playerAction.Index;

			_actionMediator.Resolve(p * success, r, i, usedSkills);
			_actionMediator.Resolve(p * (1 - success), r, i, usedSkills, true);
		}
	}
}