using ActionCalculator.Abstractions;
using ActionCalculator.ActionBuilders;
using ActionCalculator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCalculator
{
    public class ActionBuilderFactory : IActionBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ActionBuilderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IActionBuilder GetActionBuilder(string input) => GetActionBuilder(GetActionType(input));

        public IActionBuilder GetActionBuilder(ActionType actionType)
        {
            IActionBuilder? actionBuilder = actionType switch
            {
                ActionType.ArgueTheCall => _serviceProvider.GetService<ArgueTheCallBuilder>(),
                ActionType.ArmourBreak => _serviceProvider.GetService<ArmourBreakBuilder>(),
                ActionType.Bribe => _serviceProvider.GetService<BribeBuilder>(),
                ActionType.Catch => _serviceProvider.GetService<CatchBuilder>(),
                ActionType.Chainsaw => _serviceProvider.GetService<ChainsawBuilder>(),
                ActionType.Dauntless => _serviceProvider.GetService<DauntlessBuilder>(),
                ActionType.Dodge => _serviceProvider.GetService<DodgeBuilder>(),
                ActionType.Foul => _serviceProvider.GetService<FoulBuilder>(),
                ActionType.HailMaryPass => _serviceProvider.GetService<HailMaryPassBuilder>(),
                ActionType.Hypnogaze => _serviceProvider.GetService<HypnogazeBuilder>(),
                ActionType.Injury => _serviceProvider.GetService<InjuryBuilder>(),
                ActionType.Interception => _serviceProvider.GetService<InterceptionBuilder>(),
                ActionType.Landing => _serviceProvider.GetService<LandingBuilder>(),
                ActionType.NonRerollable => _serviceProvider.GetService<NonRerollableBuilder>(),
                ActionType.Pass => _serviceProvider.GetService<PassActionBuilder>(),
                ActionType.PickUp => _serviceProvider.GetService<PickupBuilder>(),
                ActionType.Rerollable => _serviceProvider.GetService<RerollableBuilder>(),
                ActionType.Rush => _serviceProvider.GetService<RushBuilder>(),
                ActionType.Shadowing => _serviceProvider.GetService<ShadowingBuilder>(),
                ActionType.Stab => _serviceProvider.GetService<StabBuilder>(),
                ActionType.Tentacles => _serviceProvider.GetService<TentaclesBuilder>(),
                ActionType.ThrowTeammate => _serviceProvider.GetService<ThrowTeammateBuilder>(),
                ActionType.Block => _serviceProvider.GetService<BlockBuilder>(),
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
