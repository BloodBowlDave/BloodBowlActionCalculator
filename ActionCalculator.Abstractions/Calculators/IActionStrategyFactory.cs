using ActionCalculator.Models;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Abstractions.Calculators
{
    public interface IActionStrategyFactory
    {
        IActionStrategy GetActionStrategy(Action action, IActionMediator actionMediator, ActionType? previousActionType, bool nonCriticalFailure);
    }
}