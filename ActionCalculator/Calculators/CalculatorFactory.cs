using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Calculators.BallHandling;
using ActionCalculator.Calculators.Blocking;
using ActionCalculator.Calculators.Fouling;
using ActionCalculator.Calculators.Movement;

namespace ActionCalculator.Calculators
{
    public class CalculatorFactory : ICalculatorFactory
    {
        public ICalculator CreateProbabilityCalculator(ActionType actionType, int blockDice,
	        ICalculator calculator, bool inaccuratePass) =>
            actionType switch
            {
                ActionType.Other => new RerollableActionCalculator(calculator, new ProCalculator()),
                ActionType.Dodge => new DodgeCalculator(calculator, new ProCalculator()),
                ActionType.Rush => new RushCalculator(calculator, new ProCalculator()),
                ActionType.PickUp => new PickUpCalculator(calculator, new ProCalculator()),
                ActionType.Pass => new PassCalculator(calculator, new ProCalculator()),
                ActionType.Block => blockDice switch
                {
                    -3 => new ThirdDieBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator()),
                    -2 => new HalfDieBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator()),
                    1 => new OneDieBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator()),
                    2 => new TwoDiceBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator()),
                    3 => new ThreeDiceBlockCalculator(calculator, new ProCalculator(), new BrawlerCalculator()),
                    _ => throw new ArgumentOutOfRangeException(nameof(blockDice), blockDice, null)
                },
                ActionType.Catch => inaccuratePass 
	                ? new CatchInaccuratePassCalculator(calculator, new ProCalculator()) 
	                : new CatchCalculator(calculator, new ProCalculator()),
                ActionType.Foul => new FoulCalculator(calculator, new TwoD6()),
                ActionType.ArmourBreak => new ArmourBreakCalculator(calculator, new TwoD6()),
                ActionType.OtherNonRerollable => new RerollableActionCalculator(calculator, new ProCalculator()),
                ActionType.ThrowTeamMate => new RerollableActionCalculator(calculator, new ProCalculator()),
                ActionType.Dauntless => new DauntlessCalculator(calculator, new ProCalculator()),
                ActionType.Interception => new InterceptionCalculator(calculator),
                ActionType.Tentacles => new TentaclesCalculator(calculator, new ProCalculator()),
                ActionType.Shadowing => new ShadowingCalculator(calculator, new ProCalculator()),
                ActionType.ArgueTheCall => new ArgueTheCallCalculator(calculator),
                ActionType.Bribe => new BribeCalculator(calculator),
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
            };
    }
}
