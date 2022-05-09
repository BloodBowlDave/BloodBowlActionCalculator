using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Calculators.BallHandling;
using ActionCalculator.Calculators.Blocking;
using ActionCalculator.Calculators.Fouling;
using ActionCalculator.Calculators.Movement;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.Calculators
{
    public class CalculatorFactory : ICalculatorFactory
    {
        public IActionStrategy GetActionStrategy(Action action, IActionMediator actionMediator, bool nonCriticalFailure) =>
            action.ActionType switch
            {
                ActionType.Rerollable => new RerollableStrategy(actionMediator, new ProCalculator()),
                ActionType.Dodge => new DodgeStrategy(actionMediator, new ProCalculator()),
                ActionType.Rush => new RushActionStrategy(actionMediator, new ProCalculator()),
                ActionType.PickUp => new PickUpStrategy(actionMediator, new ProCalculator()),
                ActionType.Pass => new PassStrategy(actionMediator, new ProCalculator()),
                ActionType.ThrowTeamMate => new PassStrategy(actionMediator, new ProCalculator()),
                ActionType.Block => action.NumberOfDice switch
                {
                    -3 => new ThirdDieBlockStrategy(actionMediator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    -2 => new HalfDieBlockStrategy(actionMediator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    1 => new OneDieBlockStrategy(actionMediator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    2 => new TwoDiceBlockStrategy(actionMediator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    3 => new ThreeDiceBlockStrategy(actionMediator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    _ => throw new ArgumentOutOfRangeException(nameof(action.NumberOfDice), action.NumberOfDice, null)
                },
                ActionType.Catch => nonCriticalFailure
                    ? new CatchInaccuratePassStrategy(actionMediator, new ProCalculator())
                    : new CatchStrategy(actionMediator, new ProCalculator()),
                ActionType.Foul => new FoulStrategy(actionMediator, new TwoD6()),
                ActionType.ArmourBreak => new ArmourBreakStrategy(actionMediator, new TwoD6()),
                ActionType.NonRerollable => new NonRerollableStrategy(actionMediator),
                ActionType.Dauntless => new DauntlessStrategy(actionMediator, new ProCalculator()),
                ActionType.Interception => new InterceptionStrategy(actionMediator),
                ActionType.Tentacles => new TentaclesActionStrategy(actionMediator, new ProCalculator()),
                ActionType.Shadowing => new ShadowingActionStrategy(actionMediator, new ProCalculator()),
                ActionType.ArgueTheCall => new ArgueTheCallStrategy(actionMediator),
                ActionType.Bribe => new BribeStrategy(actionMediator),
                ActionType.Injury => new InjuryStrategy(actionMediator, new TwoD6()),
                ActionType.Landing => new LandingStrategy(actionMediator, new ProCalculator()),
                ActionType.HailMaryPass => new HailMaryPassStrategy(actionMediator, new ProCalculator()),
                ActionType.Hypnogaze => new HypnogazeStrategy(actionMediator, new ProCalculator()),
                _ => throw new ArgumentOutOfRangeException(nameof(action.ActionType), action.ActionType, null)
            };
    }
}
