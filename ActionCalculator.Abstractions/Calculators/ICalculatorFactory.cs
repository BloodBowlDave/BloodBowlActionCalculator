namespace ActionCalculator.Abstractions.Calculators
{
    public interface ICalculatorFactory
    {
        ICalculator CreateProbabilityCalculator(ActionType actionType, int blockDice,
	        ICalculator calculator, bool inaccuratePass);
    }
}