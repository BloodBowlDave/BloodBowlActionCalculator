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
                ActionType.Block => new BlockBuilder(),
                ActionType.Foul => new FoulBuilder(),
                ActionType.ArmourBreak => new ArmourBreakBuilder(),
                ActionType.Pass => new PassActionBuilder(),
                ActionType.ThrowTeamMate => new ThrowTeamMateBuilder(),
                ActionType.Bribe => new BribeBuilder(),
                ActionType.ArgueTheCall => new ArgueTheCallBuilder(),
                ActionType.Tentacles => new TentaclesBuilder(),
                ActionType.Shadowing => new ShadowingBuilder(),
                ActionType.Injury => new InjuryBuilder(),
                ActionType.Catch => new CatchBuilder(),
                ActionType.Rerollable => new RerollableActionBuilder(),
                ActionType.Dodge => new DodgeBuilder(),
                ActionType.Rush => new RushBuilder(),
                ActionType.PickUp => new PickupBuilder(),
                ActionType.NonRerollable => new NonRerollableActionBuilder(new D6()),
                ActionType.Dauntless => new DauntlessBuilder(),
                ActionType.Interception => new InterceptionBuilder(),
                ActionType.Landing => new LandingBuilder(),
                ActionType.HailMaryPass => new HailMaryPassBuilder(),
                ActionType.Hypnogaze => new HypnogazeBuilder(),
                ActionType.Stab => new StabBuilder(),
                ActionType.Chainsaw => new ChainsawBuilder(),
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
