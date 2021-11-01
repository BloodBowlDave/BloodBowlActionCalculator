using System.Collections.Generic;

namespace ActionCalculator.Abstractions
{
	public interface IProbabilityAggregator
	{
		decimal Aggregate(IReadOnlyList<decimal[]> playerResults, int specifiedRerolls);
	}
}