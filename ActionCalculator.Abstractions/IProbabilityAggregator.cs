using System.Collections.Generic;

namespace ActionCalculator.BB3
{
	public interface IProbabilityAggregator
	{
		decimal Aggregate(IReadOnlyList<decimal[]> playerResults, int specifiedRerolls);
	}
}