using ActionCalculator.Abstractions;
using ActionCalculator.ActionBuilders;
using ActionCalculator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCalculator
{
    public class ActionParserFactory(IServiceProvider serviceProvider) : IActionParserFactory
    {
        public IActionParser GetActionParser(string input) 
        {
            var actionType = GetActionType(input);

            IActionParser? actionBuilder = actionType switch
            {
                ActionType.ArgueTheCall => serviceProvider.GetService<ArgueTheCallParser>(),
                ActionType.ArmourBreak => serviceProvider.GetService<ArmourBreakParser>(),
                ActionType.Bribe => serviceProvider.GetService<BribeParser>(),
                ActionType.Catch => serviceProvider.GetService<CatchParser>(),
                ActionType.Chainsaw => serviceProvider.GetService<ChainsawParser>(),
                ActionType.Dauntless => serviceProvider.GetService<DauntlessParser>(),
                ActionType.Dodge => serviceProvider.GetService<DodgeParser>(),
                ActionType.Foul => serviceProvider.GetService<FoulParser>(),
                ActionType.HailMaryPass => serviceProvider.GetService<HailMaryPassParser>(),
                ActionType.Hypnogaze => serviceProvider.GetService<HypnogazeParser>(),
                ActionType.Injury => serviceProvider.GetService<InjuryParser>(),
                ActionType.Interference => serviceProvider.GetService<InterferenceParser>(),
                ActionType.Leap => serviceProvider.GetService<LeapParser>(),
                ActionType.Landing => serviceProvider.GetService<LandingParser>(),
                ActionType.NonRerollable => serviceProvider.GetService<NonRerollableParser>(),
                ActionType.Pass => serviceProvider.GetService<PassParser>(),
                ActionType.PickUp => serviceProvider.GetService<PickupParser>(),
                ActionType.Rerollable => serviceProvider.GetService<RerollableParser>(),
                ActionType.Rush => serviceProvider.GetService<RushParser>(),
                ActionType.Shadowing => serviceProvider.GetService<ShadowingParser>(),
                ActionType.Stab => serviceProvider.GetService<StabParser>(),
                ActionType.Tentacles => serviceProvider.GetService<TentaclesParser>(),
                ActionType.ThrowTeammate => serviceProvider.GetService<ThrowTeammateParser>(),
                ActionType.Block => serviceProvider.GetService<BlockParser>(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (actionBuilder == null)
            {
                throw new Exception($"Action builder service not registered for type {actionType}");
            }

            return actionBuilder;
        }

        private static ActionType GetActionType(string input) =>
            IsBlockAction(input)
                ? ActionType.Block
                : input.Contains("/") 
                    ? ActionType.NonRerollable 
                    : Enum.IsDefined(typeof(ActionType), (int)input[0])
                        ? (ActionType)input[0]
                        : ActionType.Rerollable;

        private static bool IsBlockAction(string input) => input.Skip(1).Contains('D');
    }
}
