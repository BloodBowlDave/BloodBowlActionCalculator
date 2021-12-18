namespace ActionCalculator.Abstractions
{
    public interface IProbabilityCalculatorFactory
    {
        IProbabilityCalculator CreateProbabilityCalculator(ActionType actionType, int blockDice,
	        IProbabilityCalculator baseProbabilityCalculator, bool inaccuratePass);
    }
}