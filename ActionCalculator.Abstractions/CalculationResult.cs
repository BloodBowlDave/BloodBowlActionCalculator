using System.Collections.Generic;

namespace ActionCalculator.Abstractions
{
	public class CalculationResult
	{
		public CalculationResult(List<ProbabilityResult> probabilityResults)
		{
			ProbabilityResults = probabilityResults;
		}

		public List<ProbabilityResult> ProbabilityResults { get; }
	}
}