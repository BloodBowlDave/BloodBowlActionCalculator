using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
	public class ActionCalculator : IActionCalculator
	{
		private readonly ICalculationBuilder _calculationBuilder;
		private readonly IProbabilityCalculator _probabilityCalculator;
		
		public ActionCalculator(
			ICalculationBuilder calculationBuilder, 
			IProbabilityCalculator probabilityCalculator)
		{
			_calculationBuilder = calculationBuilder;
			_probabilityCalculator = probabilityCalculator;
		}

		public CalculationResult Calculate(string calculationString)
		{
			var calculation = _calculationBuilder.Build(calculationString);
			
			var probabilityResults = _probabilityCalculator.Calculate(calculation)
                .Where(x => x != null).ToList();

            foreach (var probabilityResult in probabilityResults)
			{
                AggregateResults(probabilityResult.Probabilities);
            }

            return new CalculationResult(probabilityResults);
		}

		private static void AggregateResults(IList<decimal> result)
		{
			for (var i = 1; i < result.Count; i++)
			{
				result[i] += result[i - 1];
			}
		}
	}
}