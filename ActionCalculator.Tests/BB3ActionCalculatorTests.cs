using ActionCalculator.Abstractions;
using Xunit;

namespace ActionCalculator.Tests
{
    public class BB3ActionCalculatorTests
    {
        private readonly IActionCalculator _actionCalculator;

        public BB3ActionCalculatorTests()
        {
            var actionCalculatorFactory = new ActionCalculatorFactory();
            _actionCalculator = actionCalculatorFactory.Create(Ruleset.BB3);
        }

        [Theory]
        [InlineData("2", 0.83333, 0.97222)]
        [InlineData("2,2", 0.69444, 0.92593, 0.94522)]
        [InlineData("D2,D2:D", 0.92593, 0.94522)]
        [InlineData("5", 0.33333, 0.55556)]
        [InlineData("2,3,4,5,6", 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("1D5", 0.83333, 0.97222)]
        [InlineData("2D5", 0.97222, 0.99923)]
        [InlineData("-3D1", 0.00463, 0.00924)]
        [InlineData("-2D2,2,2D3", 0.06944, 0.16011, 0.18872, 0.19129)]
        [InlineData("(2,3,4,5,6)", 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("(2,3)(4,5,6)", 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("(U2,R2,R2,R2:SH,SF)", 0.84394, 0.89083, 0.89343)]
        [InlineData("C3:C", 0.88889)]
        [InlineData("2,2:L4", 0.69444, 0.81019, 0.81501)]
		[InlineData("2,2,2,2:L3", 0.48225, 0.69659, 0.73231, 0.73496, 0.73503)]
		[InlineData("(P2:PA)(C2)", 0.81019, 0.94522)]
		//[InlineData("(P4-2)(C2)", 0.13889, 0.27778, 0.29707)]
		[InlineData("(I4-2,N24/512)(C3)", 0.01042, 0.01910, 0.02199)]
        public void ActionCalculatorReturnsExpectedResult(string calculation, params double[] expected)
        {
	        var result = _actionCalculator.Calculate(calculation);

            Assert.Equal(expected.Length, result.Probabilities.Count);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal((decimal) expected[i], result.Probabilities[i], 5);
            }
        }
    }
}
