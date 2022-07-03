using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Strategies;
using ActionCalculator.Strategies.BallHandling;
using ActionCalculator.Strategies.Blocking;
using ActionCalculator.Strategies.Fouling;
using ActionCalculator.Strategies.Movement;
using Action = ActionCalculator.Models.Actions.Action;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCalculator
{
    public class StrategyFactory : IStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IActionStrategy GetActionStrategy(Action action, IActionMediator actionMediator, ActionType? previousActionType, bool nonCriticalFailure)
        {
            var actionType = action.ActionType;

            IActionStrategy? actionStrategy = actionType switch
            {
                ActionType.ArgueTheCall => _serviceProvider.GetService<ArgueTheCallStrategy>(),
                ActionType.ArmourBreak => _serviceProvider.GetService<ArmourBreakStrategy>(),
                ActionType.Block => ((Block)action).NumberOfDice > 0 
                    ? _serviceProvider.GetService<BlockStrategy>() 
                    : _serviceProvider.GetService<RedDiceBlockStrategy>(),
                ActionType.Bribe => _serviceProvider.GetService<BribeStrategy>(),
                ActionType.Catch => nonCriticalFailure 
                    ? _serviceProvider.GetService<CatchInaccuratePassStrategy>() 
                    : _serviceProvider.GetService<CatchStrategy>(),
                ActionType.Chainsaw => _serviceProvider.GetService<ChainsawStrategy>(),
                ActionType.Dauntless => _serviceProvider.GetService<DauntlessStrategy>(),
                ActionType.Dodge => _serviceProvider.GetService<DodgeStrategy>(),
                ActionType.Foul => _serviceProvider.GetService<FoulStrategy>(),
                ActionType.HailMaryPass => _serviceProvider.GetService<HailMaryPassStrategy>(),
                ActionType.Hypnogaze => _serviceProvider.GetService<HypnogazeStrategy>(),
                ActionType.Injury => previousActionType == ActionType.Foul 
                    ? _serviceProvider.GetService<FoulInjuryStrategy>() 
                    : _serviceProvider.GetService<InjuryStrategy>(),
                ActionType.Interception => _serviceProvider.GetService<InterceptionStrategy>(),
                ActionType.Landing => _serviceProvider.GetService<LandingStrategy>(),
                ActionType.NonRerollable => _serviceProvider.GetService<NonRerollableStrategy>(),
                ActionType.Pass => _serviceProvider.GetService<PassStrategy>(),
                ActionType.PickUp => _serviceProvider.GetService<PickUpStrategy>(),
                ActionType.Rush => _serviceProvider.GetService<RushStrategy>(),
                ActionType.Shadowing => _serviceProvider.GetService<ShadowingStrategy>(),
                ActionType.Stab => _serviceProvider.GetService<StabStrategy>(),
                ActionType.Tentacles => _serviceProvider.GetService<TentaclesStrategy>(),
                ActionType.ThrowTeammate => _serviceProvider.GetService<ThrowTeammateStrategy>(),
                ActionType.Rerollable => _serviceProvider.GetService<RerollableStrategy>(),
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
            };

            if (actionStrategy == null)
            {
                throw new Exception($"Strategy service not registered for type {actionType}");
            }

            return actionStrategy;
        }
    }
}
