using ActionCalculator.Abstractions;
using ActionCalculator.ActionBuilders;
using ActionCalculator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCalculator
{
    public class ActionParserFactory : IActionParserFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ActionParserFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IActionParser GetActionParser(string input) => GetActionParser(GetActionType(input));

        public IActionParser GetActionParser(ActionType actionType)
        {
            IActionParser? actionBuilder = actionType switch
            {
                ActionType.ArgueTheCall => _serviceProvider.GetService<ArgueTheCallParser>(),
                ActionType.ArmourBreak => _serviceProvider.GetService<ArmourBreakParser>(),
                ActionType.Bribe => _serviceProvider.GetService<BribeParser>(),
                ActionType.Catch => _serviceProvider.GetService<CatchParser>(),
                ActionType.Chainsaw => _serviceProvider.GetService<ChainsawParser>(),
                ActionType.Dauntless => _serviceProvider.GetService<DauntlessParser>(),
                ActionType.Dodge => _serviceProvider.GetService<DodgeParser>(),
                ActionType.Foul => _serviceProvider.GetService<FoulParser>(),
                ActionType.HailMaryPass => _serviceProvider.GetService<HailMaryPassParser>(),
                ActionType.Hypnogaze => _serviceProvider.GetService<HypnogazeParser>(),
                ActionType.Injury => _serviceProvider.GetService<InjuryParser>(),
                ActionType.Interference => _serviceProvider.GetService<InterferenceParser>(),
                ActionType.Landing => _serviceProvider.GetService<LandingParser>(),
                ActionType.NonRerollable => _serviceProvider.GetService<NonRerollableParser>(),
                ActionType.Pass => _serviceProvider.GetService<PassParser>(),
                ActionType.PickUp => _serviceProvider.GetService<PickupParser>(),
                ActionType.Rerollable => _serviceProvider.GetService<RerollableParser>(),
                ActionType.Rush => _serviceProvider.GetService<RushParser>(),
                ActionType.Shadowing => _serviceProvider.GetService<ShadowingParser>(),
                ActionType.Stab => _serviceProvider.GetService<StabParser>(),
                ActionType.Tentacles => _serviceProvider.GetService<TentaclesParser>(),
                ActionType.ThrowTeammate => _serviceProvider.GetService<ThrowTeammateParser>(),
                ActionType.Block => _serviceProvider.GetService<BlockParser>(),
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
                : Enum.IsDefined(typeof(ActionType), (int)input[0])
                    ? (ActionType)input[0]
                    : ActionType.Rerollable;

        private static bool IsBlockAction(string input) => input.Skip(1).Contains('D');
    }
}
