using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
	public class ProbabilityAggregator : IProbabilityAggregator
	{
		public decimal Aggregate(IReadOnlyList<decimal[]> playerResults, int specifiedRerolls)
		{
			var playerIndices = new List<int>();

			foreach (var playerResult in playerResults)
			{
				playerIndices.AddRange(Enumerable.Range(0, playerResult.Length));
			}

			var indicesCombination = GetIndicesWhereSumToTarget(playerIndices, specifiedRerolls, playerResults.Count);
            
			var playerStartIndices = new List<int>();

			for (var i = 0; i < playerIndices.Count; i++)
			{
				if (playerIndices[i] == 0)
				{
					playerStartIndices.Add(i);
				}
			}

			indicesCombination = playerStartIndices
				.Select((_, i) => i)
				.Aggregate<int, IEnumerable<int[]>>(indicesCombination, (current, index) =>
					current.Where(x => 
						x.Count(y => 
							y >= playerStartIndices[index]) == playerStartIndices.Count - index));

			var sum = 0m;

			foreach (var indices in indicesCombination)
			{
				var p = 1m;

				for (var i = 0; i < indices.Length; i++)
				{
					p *= playerResults[i][indices[i] - playerStartIndices[i]];
				}

				sum += p;
			}

			return sum;
		}

		private static IEnumerable<int[]> GetIndicesWhereSumToTarget(IReadOnlyList<int> indices, int target, int indicesCount)
		{
			var output = new List<int[]>();

			var combinations = (int)System.Math.Pow(2, indices.Count);

			for (var i = 0; i < combinations; i++)
			{
				var sum = 0;
				var subIndices = new List<int>();

				for (var j = 0; j < indices.Count; j++)
				{
					if ((i & (1 << j)) >> j != 1 || sum > target)
					{
						continue;
					}

					sum += indices[j];
                    
					subIndices.Add(j);
				}

				if (sum == target && subIndices.Count == indicesCount)
				{
					output.Add(subIndices.ToArray());
				}
			}

			return output;
		}
	}
}