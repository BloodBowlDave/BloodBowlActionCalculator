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
        public ICalculator CreateProbabilityCalculator(Action action, ICalculator calculator, bool nonCriticalFailure) =>
            action.ActionType switch
            {
                ActionType.Rerollable => new RerollableActionCalculator(calculator, new ProCalculator()),
                ActionType.Dodge => new DodgeCalculator(calculator, new ProCalculator()),
                ActionType.Rush => new RushCalculator(calculator, new ProCalculator()),
                ActionType.PickUp => new PickUpCalculator(calculator, new ProCalculator()),
                ActionType.Pass => new PassCalculator(calculator, new ProCalculator()),
                ActionType.ThrowTeamMate => new PassCalculator(calculator, new ProCalculator()),
                ActionType.Block => action.NumberOfDice switch
                {
                    -3 => new ThirdDieBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    -2 => new HalfDieBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    1 => new OneDieBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    2 => new TwoDiceBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    3 => new ThreeDiceBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator(new ProCalculator())),
                    _ => throw new ArgumentOutOfRangeException(nameof(action.NumberOfDice), action.NumberOfDice, null)
                },
                ActionType.Catch => nonCriticalFailure 
	                ? new CatchInaccuratePassCalculator(calculator, new ProCalculator()) 
	                : new CatchCalculator(calculator, new ProCalculator()),
                ActionType.Foul => new FoulCalculator(calculator, new TwoD6()),
                ActionType.ArmourBreak => new ArmourBreakCalculator(calculator, new TwoD6()),
                ActionType.NonRerollable => new NonRerollableActionCalculator(calculator),
                ActionType.Dauntless => new DauntlessCalculator(calculator, new ProCalculator()),
                ActionType.Interception => new InterceptionCalculator(calculator),
                ActionType.Tentacles => new TentaclesCalculator(calculator, new ProCalculator()),
                ActionType.Shadowing => new ShadowingCalculator(calculator, new ProCalculator()),
                ActionType.ArgueTheCall => new ArgueTheCallCalculator(calculator),
                ActionType.Bribe => new BribeCalculator(calculator),
                ActionType.Injury => new InjuryCalculator(calculator, new TwoD6()),
                ActionType.Landing => new LandingCalculator(calculator, new ProCalculator()),
                ActionType.HailMaryPass => new HailMaryPassCalculator(calculator, new ProCalculator()),
                ActionType.Hypnogaze => new HypnogazeCalculator(calculator, new ProCalculator()),
                _ => throw new ArgumentOutOfRangeException(nameof(action.ActionType), action.ActionType, null)
            };
    }
}
