using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Strategies;
using ActionCalculator.Strategies.BallHandling;
using ActionCalculator.Strategies.Blocking;
using ActionCalculator.Strategies.Fouling;
using ActionCalculator.Strategies.Movement;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
    public class ActionStrategyFactory : IActionStrategyFactory
    {
        public IActionStrategy GetActionStrategy(Action action, IActionMediator actionMediator, bool nonCriticalFailure) =>
            action.ActionType switch
            {
                ActionType.Rerollable => new RerollableStrategy(actionMediator, new ProHelper()),
                ActionType.Dodge => new DodgeStrategy(actionMediator, new ProHelper()),
                ActionType.Rush => new RushActionStrategy(actionMediator, new ProHelper()),
                ActionType.PickUp => new PickUpStrategy(actionMediator, new ProHelper()),
                ActionType.Pass => new PassStrategy(actionMediator, new ProHelper()),
                ActionType.ThrowTeamMate => new PassStrategy(actionMediator, new ProHelper()),
                ActionType.Block => action.NumberOfDice switch
                {
                    -3 => new ThirdDieBlockStrategy(actionMediator, new ProHelper(), new BrawlerHelper(new ProHelper())),
                    -2 => new HalfDieBlockStrategy(actionMediator, new ProHelper(), new BrawlerHelper(new ProHelper())),
                    1 => new OneDieBlockStrategy(actionMediator, new ProHelper(), new BrawlerHelper(new ProHelper())),
                    2 => new TwoDiceBlockStrategy(actionMediator, new ProHelper(), new BrawlerHelper(new ProHelper())),
                    3 => new ThreeDiceBlockStrategy(actionMediator, new ProHelper(), new BrawlerHelper(new ProHelper())),
                    _ => throw new ArgumentOutOfRangeException(nameof(action.NumberOfDice), action.NumberOfDice, null)
                },
                ActionType.Catch => nonCriticalFailure
                    ? new CatchInaccuratePassStrategy(actionMediator, new ProHelper())
                    : new CatchStrategy(actionMediator, new ProHelper()),
                ActionType.Foul => new FoulStrategy(actionMediator, new TwoD6()),
                ActionType.ArmourBreak => new ArmourBreakStrategy(actionMediator, new TwoD6()),
                ActionType.NonRerollable => new NonRerollableStrategy(actionMediator),
                ActionType.Dauntless => new DauntlessStrategy(actionMediator, new ProHelper()),
                ActionType.Interception => new InterceptionStrategy(actionMediator),
                ActionType.Tentacles => new TentaclesActionStrategy(actionMediator, new ProHelper()),
                ActionType.Shadowing => new ShadowingActionStrategy(actionMediator, new ProHelper()),
                ActionType.ArgueTheCall => new ArgueTheCallStrategy(actionMediator),
                ActionType.Bribe => new BribeStrategy(actionMediator),
                ActionType.Injury => new InjuryStrategy(actionMediator, new TwoD6()),
                ActionType.Landing => new LandingStrategy(actionMediator, new ProHelper()),
                ActionType.HailMaryPass => new HailMaryPassStrategy(actionMediator, new ProHelper()),
                ActionType.Hypnogaze => new HypnogazeStrategy(actionMediator, new ProHelper()),
                _ => throw new ArgumentOutOfRangeException(nameof(action.ActionType), action.ActionType, null)
            };
    }
}
