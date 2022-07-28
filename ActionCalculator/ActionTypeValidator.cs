using ActionCalculator.Abstractions;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class ActionTypeValidator : IActionTypeValidator
    {
        public bool ActionTypeIsValid(ActionType actionType, ActionType? previousActionType, ActionType? previousPreviousActionType)
        {
            var previousDauntless = previousActionType is ActionType.Dauntless || previousPreviousActionType is ActionType.Dauntless;

            return actionType switch
            {
                ActionType.Rerollable => !previousDauntless,
                ActionType.Dodge => !previousDauntless,
                ActionType.Rush => !previousDauntless,
                ActionType.PickUp => !previousDauntless,
                ActionType.Pass => !previousDauntless,
                ActionType.Block => true,
                ActionType.Catch => !previousDauntless,
                ActionType.Foul => !previousDauntless,
                ActionType.ArmourBreak => !previousDauntless && previousActionType is ActionType.Block or ActionType.Foul,
                ActionType.NonRerollable => !previousDauntless,
                ActionType.ThrowTeammate => !previousDauntless,
                ActionType.Dauntless => !previousDauntless,
                ActionType.Interference => !previousDauntless && previousActionType is ActionType.Pass,
                ActionType.Tentacles => !previousDauntless,
                ActionType.Shadowing => !previousDauntless,
                ActionType.ArgueTheCall => !previousDauntless && (previousActionType is ActionType.Foul
                                                                  || previousActionType is ActionType.Injury && previousPreviousActionType is ActionType.Foul
                                                                  || previousActionType is ActionType.Bribe),
                ActionType.Bribe => !previousDauntless && (previousActionType is ActionType.Foul
                                                           || previousActionType is ActionType.Injury &&
                                                           previousPreviousActionType is ActionType.Foul
                                                           || previousActionType is ActionType.Bribe or ActionType.ArgueTheCall),
                ActionType.Injury => !previousDauntless && previousActionType is ActionType.ArmourBreak or ActionType.Foul,
                ActionType.Landing => !previousDauntless && previousActionType is ActionType.ThrowTeammate,
                ActionType.HailMaryPass => !previousDauntless,
                ActionType.Hypnogaze => !previousDauntless,
                ActionType.Stab => !previousDauntless,
                ActionType.Chainsaw => !previousDauntless,
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
            };
        }
    }
}
