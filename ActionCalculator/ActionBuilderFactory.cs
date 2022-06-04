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
                ActionType.Pass => new PassActionBuilder(new D6()),
                ActionType.ThrowTeamMate => new ThrowTeamMateBuilder(new D6()),
                ActionType.Bribe => new BribeBuilder(),
                ActionType.ArgueTheCall => new ArgueTheCallBuilder(new D6()),
                ActionType.Tentacles => new TentaclesBuilder(),
                ActionType.Shadowing => new ShadowingBuilder(),
                ActionType.Injury => new InjuryBuilder(),
                ActionType.Catch => new CatchBuilder(new D6()),
                ActionType.Rerollable => new RerollableActionBuilder(new D6()),
                ActionType.Dodge => new DodgeBuilder(new D6()),
                ActionType.Rush => new RushBuilder(new D6()),
                ActionType.PickUp => new PickupBuilder(new D6()),
                ActionType.NonRerollable => new NonRerollableActionBuilder(new D6()),
                ActionType.Dauntless => new DauntlessBuilder(new D6()),
                ActionType.Interception => new InterceptionBuilder(new D6()),
                ActionType.Landing => new LandingBuilder(new D6()),
                ActionType.HailMaryPass => new HailMaryPassBuilder(new D6()),
                ActionType.Hypnogaze => new HypnogazeBuilder(new D6()),
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
