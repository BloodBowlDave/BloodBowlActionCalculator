namespace ActionCalculator.Abstractions
{
	public interface IBaseProbabilityCalculator : IProbabilityCalculator
	{
		void Initialise(CalculationContext context);
	}
}
