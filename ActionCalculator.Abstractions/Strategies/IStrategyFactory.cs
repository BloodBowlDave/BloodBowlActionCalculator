using ActionCalculator.Models;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Abstractions.Strategies
{
	public interface IStrategyFactory
    {
        IActionStrategy GetActionStrategy(Action action, IActionMediator actionMediator, ActionType? previousActionType, bool nonCriticalFailure);
    }
}