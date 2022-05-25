using ActionCalculator.Abstractions;
using ActionCalculator.ActionBuilders;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class ActionBuilderFactory : IActionBuilderFactory
    {
        public IActionBuilder GetActionBuilder(string input) =>
            GetActionType(input) switch
            {
                ActionType.Block => new BlockActionBuilder(),
                ActionType.Foul => new FoulActionBuilder(),
                ActionType.ArmourBreak => new ArmourBreakActionBuilder(),
                ActionType.Pass => new PassActionBuilder(),
                ActionType.ThrowTeamMate => new ThrowTeamMateActionBuilder(),
                ActionType.Bribe => new BribeActionBuilder(),
                ActionType.ArgueTheCall => new ArgueTheCallActionBuilder(),
                ActionType.Tentacles => new TentaclesActionBuilder(),
                ActionType.Shadowing => new ShadowingActionBuilder(),
                ActionType.Injury => new InjuryActionBuilder(),
                ActionType.Catch => new CatchActionBuilder(),
                ActionType.Rerollable => new RerollableActionBuilder(),
                ActionType.Dodge => new DodgeActionBuilder(),
                ActionType.Rush => new RushActionBuilder(),
                ActionType.PickUp => new PickupActionBuilder(),
                ActionType.NonRerollable => new NonRerollableActionBuilder(),
                ActionType.Dauntless => new DauntlessActionBuilder(),
                ActionType.Interception => new InterceptionActionBuilder(),
                ActionType.Landing => new LandingActionBuilder(),
                ActionType.HailMaryPass => new HailMaryPassActionBuilder(),
                ActionType.Hypnogaze => new HypnogazeActionBuilder(),
                _ => throw new ArgumentOutOfRangeException()
            };

        private static ActionType GetActionType(string input) =>
            IsBlockAction(input)
                ? ActionType.Block
                : Enum.IsDefined(typeof(ActionType), (int)input[0])
                    ? (ActionType)input[0]
                    : ActionType.Rerollable;

        private static bool IsBlockAction(string input) => input.Skip(1).Contains('D');
    }
}
