using System.Collections.Generic;
using System.Linq;

namespace ActionCalculator
{
	public static class ListExtensions
	{
		public static IEnumerable<List<T>> GetAllCombinations<T>(this IReadOnlyList<T> list)
		{
			var result = new List<List<T>>();

			for (var i = 0; i < (int)System.Math.Pow(2, list.Count); i++)
			{
				result.Add(new List<T>());

				for (var j = 0; j < list.Count; j++)
				{
					if ((i >> j) % 2 != 0)
					{
						result.Last().Add(list[j]);
					}
				}
			}

			return result;
		}
	}
}