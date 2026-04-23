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
    public class StrategyFactory(IServiceProvider serviceProvider, ICalculationContext context) : IStrategyFactory
    {
        public IActionStrategy GetActionStrategy(Action action, ICalculator calculator, bool nonCriticalFailure)
        {
            var actionType = action.ActionType;

            IActionStrategy? actionStrategy = actionType switch
            {
                ActionType.ArgueTheCall => serviceProvider.GetService<ArgueTheCallStrategy>(),
                ActionType.ArmourBreak => serviceProvider.GetService<ArmourBreakStrategy>(),
                ActionType.Block => ((Block)action).NumberOfDice > 0 
                    ? serviceProvider.GetService<BlockStrategy>() 
                    : serviceProvider.GetService<FractionalDiceBlockStrategy>(),
                ActionType.Bribe => serviceProvider.GetService<BribeStrategy>(),
                ActionType.Catch => nonCriticalFailure 
                    ? serviceProvider.GetService<CatchInaccuratePassStrategy>() 
                    : serviceProvider.GetService<CatchStrategy>(),
                ActionType.Chainsaw => serviceProvider.GetService<ChainsawStrategy>(),
                ActionType.Dauntless => serviceProvider.GetService<DauntlessStrategy>(),
                ActionType.Dodge => serviceProvider.GetService<DodgeStrategy>(),
                ActionType.Foul => serviceProvider.GetService<FoulStrategy>(),
                ActionType.HailMaryPass => serviceProvider.GetService<HailMaryPassStrategy>(),
                ActionType.Hypnogaze => serviceProvider.GetService<HypnogazeStrategy>(),
                ActionType.Injury => context.PreviousActionType == ActionType.Foul
                    ? serviceProvider.GetService<FoulInjuryStrategy>()
                    : serviceProvider.GetService<InjuryStrategy>(),
                ActionType.Interference => serviceProvider.GetService<InterferenceStrategy>(),
                ActionType.Landing => serviceProvider.GetService<LandingStrategy>(),
                ActionType.Leap => serviceProvider.GetService<LeapStrategy>(),
                ActionType.NonRerollable => serviceProvider.GetService<NonRerollableStrategy>(),
                ActionType.Pass => serviceProvider.GetService<PassStrategy>(),
                ActionType.PickUp => serviceProvider.GetService<PickUpStrategy>(),
                ActionType.Rush => serviceProvider.GetService<RushStrategy>(),
                ActionType.Shadowing => serviceProvider.GetService<ShadowingStrategy>(),
                ActionType.Stab => serviceProvider.GetService<StabStrategy>(),
                ActionType.Tentacles => serviceProvider.GetService<TentaclesStrategy>(),
                ActionType.ThrowTeammate => serviceProvider.GetService<ThrowTeammateStrategy>(),
                ActionType.Rerollable => serviceProvider.GetService<RerollableStrategy>(),
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
