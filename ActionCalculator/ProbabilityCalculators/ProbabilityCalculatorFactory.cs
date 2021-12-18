using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators;
using ActionCalculator.ProbabilityCalculators.Block;

namespace ActionCalculator.ProbabilityCalculators
{
    public class ProbabilityCalculatorFactory : IProbabilityCalculatorFactory
    {
        public IProbabilityCalculator CreateProbabilityCalculator(ActionType actionType, int blockDice,
	        IProbabilityCalculator baseProbabilityCalculator, bool inaccuratePass) =>
            actionType switch
            {
                ActionType.Other => new RerollableActionCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.Dodge => new DodgeCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.Rush => new RushCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.PickUp => new PickUpCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.Pass => new PassCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.Block => blockDice switch
                {
                    -3 => new ThirdDieBlockCalculator(baseProbabilityCalculator, new ProCalculator()),
                    -2 => new HalfDieBlockCalculator(baseProbabilityCalculator, new ProCalculator()),
                    1 => new OneDiceBlockCalculator(baseProbabilityCalculator, new ProCalculator()),
                    2 => new TwoDiceBlockCalculator(baseProbabilityCalculator, new ProCalculator(), new BrawlerCalculator()),
                    3 => new ThreeDiceBlockCalculator(baseProbabilityCalculator, new ProCalculator()),
                    _ => throw new ArgumentOutOfRangeException(nameof(blockDice), blockDice, null)
                },
                ActionType.Catch => inaccuratePass 
	                ? new CatchInaccuratePassCalculator(baseProbabilityCalculator, new ProCalculator()) 
	                : new CatchCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.Foul => new RerollableActionCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.ArmourBreak => new RerollableActionCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.Injury => new RerollableActionCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.OtherNonRerollable => new RerollableActionCalculator(baseProbabilityCalculator, new ProCalculator()),
                ActionType.ThrowTeamMate => new RerollableActionCalculator(baseProbabilityCalculator, new ProCalculator()),
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
            };
    }
}
