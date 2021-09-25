using System.Collections.Generic;

namespace ActionCalculator.Abstractions
{
	public class CalculationResult
	{
		public CalculationResult(List<decimal> probabilities)
		{
			Probabilities = probabilities;
		}

		public List<decimal> Probabilities { get; }
	}
}