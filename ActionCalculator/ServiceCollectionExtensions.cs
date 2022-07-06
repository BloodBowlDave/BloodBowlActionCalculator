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
            services.AddSingleton<IActionParserFactory, ActionParserFactory>();
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
            services.AddSingleton<ArgueTheCallParser>();
            services.AddSingleton<ArmourBreakParser>();
            services.AddSingleton<BlockParser>();
            services.AddSingleton<BribeParser>();
            services.AddSingleton<CatchParser>();
            services.AddSingleton<ChainsawParser>();
            services.AddSingleton<DauntlessParser>();
            services.AddSingleton<DodgeParser>();
            services.AddSingleton<FoulParser>();
            services.AddSingleton<HailMaryPassParser>();
            services.AddSingleton<HypnogazeParser>();
            services.AddSingleton<InjuryParser>();
            services.AddSingleton<InterceptionParser>();
            services.AddSingleton<LandingParser>();
            services.AddSingleton<NonRerollableParser>();
            services.AddSingleton<PassActionParser>();
            services.AddSingleton<PickupParser>();
            services.AddSingleton<RerollableParser>();
            services.AddSingleton<RushParser>();
            services.AddSingleton<ShadowingParser>();
            services.AddSingleton<StabParser>();
            services.AddSingleton<TentaclesParser>();
            services.AddSingleton<ThrowTeammateParser>();
        }
    }
}
