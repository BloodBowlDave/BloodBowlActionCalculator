using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Abstractions
{
	public interface IMasterCalculator : ICalculator
	{
		void Initialise(CalculationContext context);
	}
}
