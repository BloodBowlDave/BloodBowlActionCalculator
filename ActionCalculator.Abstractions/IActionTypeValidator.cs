using ActionCalculator.Models;

namespace ActionCalculator.Abstractions;

public interface IActionTypeValidator
{
    bool ActionTypeIsValid(ActionType actionType, ActionType? previousActionType, ActionType? previousPreviousActionType);
}