using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Abstractions.Strategies.Blocking;
using ActionCalculator.ActionBuilders;
using ActionCalculator.Strategies;
using ActionCalculator.Strategies.BallHandling;
using ActionCalculator.Strategies.Blocking;
using ActionCalculator.Strategies.Fouling;
using ActionCalculator.Strategies.Movement;
using ActionCalculator.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCalculator
{
    public static class ServiceCollectionExtensions
    {
        public static void AddActionCalculatorServices(this IServiceCollection services)
        {
            services.AddSingleton<IActionBuilderFactory, ActionBuilderFactory>();
            services.AddSingleton<IActionMediator, ActionMediator>();
            services.AddSingleton<IBrawlerHelper, BrawlerHelper>();
            services.AddSingleton<ICalculator, Calculator>();
            services.AddSingleton<ID6, D6>();
            services.AddSingleton<IEqualityComparer<decimal>, ProbabilityComparer>();
            services.AddSingleton<IPlayerActionsBuilder, PlayerActionsBuilder>();
            services.AddSingleton<IPlayerBuilder, PlayerBuilder>();
            services.AddSingleton<IProHelper, ProHelper>();
            services.AddSingleton<IStrategyFactory, StrategyFactory>();

            AddActionBuilders(services);
            AddActionStrategies(services);
        }

        private static void AddActionStrategies(IServiceCollection services)
        {
            services.AddSingleton<ArmourBreakStrategy>();
            services.AddSingleton<BlockStrategy>();
            services.AddSingleton<BribeStrategy>();
            services.AddSingleton<CatchInaccuratePassStrategy>();
            services.AddSingleton<CatchStrategy>();
            services.AddSingleton<ChainsawStrategy>();
            services.AddSingleton<DauntlessStrategy>();
            services.AddSingleton<DodgeStrategy>();
            services.AddSingleton<FoulInjuryStrategy>();
            services.AddSingleton<FoulStrategy>();
            services.AddSingleton<HailMaryPassStrategy>();
            services.AddSingleton<HypnogazeStrategy>();
            services.AddSingleton<InjuryStrategy>();
            services.AddSingleton<InterceptionStrategy>();
            services.AddSingleton<LandingStrategy>();
            services.AddSingleton<NonRerollableStrategy>();
            services.AddSingleton<PassStrategy>();
            services.AddSingleton<PickUpStrategy>();
            services.AddSingleton<RedDiceBlockStrategy>();
            services.AddSingleton<RerollableStrategy>();
            services.AddSingleton<RushStrategy>();
            services.AddSingleton<ShadowingStrategy>();
            services.AddSingleton<StabStrategy>();
            services.AddSingleton<TentaclesStrategy>();
            services.AddSingleton<ThrowTeammateStrategy>();
            services.AddSingleton<ArgueTheCallStrategy>();
        }

        private static void AddActionBuilders(IServiceCollection services)
        {
            services.AddSingleton<ArgueTheCallBuilder>();
            services.AddSingleton<ArmourBreakBuilder>();
            services.AddSingleton<BlockBuilder>();
            services.AddSingleton<BribeBuilder>();
            services.AddSingleton<CatchBuilder>();
            services.AddSingleton<ChainsawBuilder>();
            services.AddSingleton<DauntlessBuilder>();
            services.AddSingleton<DodgeBuilder>();
            services.AddSingleton<FoulBuilder>();
            services.AddSingleton<HailMaryPassBuilder>();
            services.AddSingleton<HypnogazeBuilder>();
            services.AddSingleton<InjuryBuilder>();
            services.AddSingleton<InterceptionBuilder>();
            services.AddSingleton<LandingBuilder>();
            services.AddSingleton<NonRerollableBuilder>();
            services.AddSingleton<PassActionBuilder>();
            services.AddSingleton<PickupBuilder>();
            services.AddSingleton<RerollableBuilder>();
            services.AddSingleton<RushBuilder>();
            services.AddSingleton<ShadowingBuilder>();
            services.AddSingleton<StabBuilder>();
            services.AddSingleton<TentaclesBuilder>();
            services.AddSingleton<ThrowTeammateBuilder>();
        }
    }
}
