using ActionCalculator.Abstractions;
using Xunit;

namespace ActionCalculator.Tests
{
	public class BB2ActionCalculatorTests
	{
		private readonly IActionCalculator _actionCalculator;

		public BB2ActionCalculatorTests()
		{
			var actionCalculatorFactory = new ActionCalculatorFactory();

			_actionCalculator = actionCalculatorFactory.Create(Ruleset.BB2);
		}

		[Theory]
		[InlineData("2", 0.83333, 0.97222)]
		[InlineData("2,2", 0.69444, 0.92593)]
		[InlineData("D2,D2:D", 0.92593, 0.94522)]
		[InlineData("5", 0.33333, 0.55556)]
		[InlineData("2,3,4,5,6", 0.01543, 0.05401)]
		[InlineData("1D5", 0.83333, 0.97222)]
		[InlineData("2D5", 0.97222, 0.99923)]
		[InlineData("-3D1", 0.00463, 0.00924)]
		[InlineData("-2D2,2,2D3", 0.06944, 0.16011)]
		[InlineData("(2,3,4,5,6)", 0.01543, 0.05401)]
		[InlineData("(2,3)(4,5,6)", 0.01543, 0.05401)]
		[InlineData("(U2,G2,G2,G2:SH,SF)", 0.84394, 0.89083)]
		[InlineData("C3:C", 0.88889)]
		[InlineData("2,2:L", 0.69444, 0.81019)]
		[InlineData("2,2:P", 0.81019, 0.93557)]
		[InlineData("2,2:P,L", 0.81019, 0.81501)]
		public void ActionCalculatorReturnsExpectedResult(string calculation, params double[] expected)
		{
			var result = _actionCalculator.Calculate(calculation);

			Assert.Equal(expected.Length, result.Probabilities.Count);

			for (var i = 0; i < expected.Length; i++)
			{
				Assert.Equal((decimal)expected[i], result.Probabilities[i], 5);
			}
		}
	}
}
