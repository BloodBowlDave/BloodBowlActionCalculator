using System;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
	public class ActionCalculatorFactory
	{
		public IActionCalculator Create(Ruleset ruleset) =>
			ruleset switch
			{
				Ruleset.BB3 => CreateBB3ActionCalculator(),
				_ => throw new ArgumentOutOfRangeException(nameof(ruleset), ruleset, null)
			};

		private static ActionCalculator CreateBB3ActionCalculator() =>
			new(new CalculationBuilder(new ActionBuilder(), new PlayerParser()),
				new ProbabilityCalculator(new ProbabilityComparer()));
		
	}
}
