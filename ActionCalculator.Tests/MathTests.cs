using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ActionCalculator.Tests
{
	public class MathTests
	{
		[Fact]
		public void Assert123123()
		{
			var combinations = new List<List<int>>();

			for (int i = 1; i <= 6; i++)
			{
				for (int j = 1; j <= 6; j++)
				{
					for (int k = 1; k <= 6; k++)
					{
						combinations.Add(new List<int> {i,j,k});
					}
				}
			}

			var containsFive = combinations.Count(x => x.Contains(5));
			var containsFiveButNotSix = combinations.Count(x => x.Contains(5) && !x.Contains(6));
			var containsFiveButNotSixOrFour = combinations.Count(x => x.Contains(5) && !x.Contains(6) && !x.Contains(4));
			var containsFiveButNotSixOrFourOrThree = combinations.Count(x => x.Contains(5) && !x.Contains(6) && !x.Contains(4) && !x.Contains(3));

			Assert.Equal(1, 1);
		}
	}
}
