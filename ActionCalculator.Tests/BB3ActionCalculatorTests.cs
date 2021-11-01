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
        [InlineData("2", 1, 0.83333, 0.97222)]
        [InlineData("2,2", 2, 0.69444, 0.92593, 0.94522)]
        [InlineData("D2,D2:D", 1, 0.92593, 0.94522)]
        [InlineData("5", 1, 0.33333, 0.55556)]
        [InlineData("2,3,4,5,6", 5, 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("1D5", 1, 0.83333, 0.97222)]
        [InlineData("2D5", 1, 0.97222, 0.99923)]
        [InlineData("-3D1", 1, 0.00463, 0.00924)]
        [InlineData("-2D2,2,2D3", 3, 0.06944, 0.16011, 0.18872, 0.19129)]
        [InlineData("(2,3,4,5,6)", 5, 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("(2,3)(4,5,6)", 5, 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("(U2,R2,R2,R2:SH,SF)", 2, 0.84394, 0.89083, 0.89343)]
        [InlineData("C3:C", 0, 0.88889)]
        [InlineData("2,2:L4", 2, 0.69444, 0.81019, 0.81501)]
		[InlineData("2,2,2,2:L3", 4, 0.48225, 0.69659, 0.73231, 0.73496, 0.73503)]
		[InlineData("(P2:PA)(C2)", 1, 0.81019, 0.94522)]
        [InlineData("(P4-2)(C2)", 2, 0.13889, 0.28646, 0.30864)]
        [InlineData("(P4-2)(C2)", 1, 0.13889, 0.28646)]
        [InlineData("(P4-2)(C2)", 0, 0.14931)]
        public void ActionCalculatorReturnsExpectedResult(string calculation, int rerolls, params double[] expected)
        {
            var result = _actionCalculator.Calculate(calculation);

            Assert.Equal(expected.Length, result.ProbabilityResults[rerolls].Probabilities.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal((decimal) expected[i], result.ProbabilityResults[rerolls].Probabilities[i], 5);
            }
        }
    }
}
