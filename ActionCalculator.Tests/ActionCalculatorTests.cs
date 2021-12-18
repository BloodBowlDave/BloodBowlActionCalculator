using ActionCalculator.Abstractions;
using ActionCalculator.ProbabilityCalculators;
using Xunit;

namespace ActionCalculator.Tests
{
    public class ActionCalculatorTests
    {
        private readonly IActionCalculator _actionCalculator;

        public ActionCalculatorTests()
        {
            _actionCalculator = new ActionCalculator(
                new CalculationBuilder(
                    new ActionBuilder(), 
                    new PlayerParser()), 
                new ProbabilityComparer(),
                new BaseProbabilityCalculator(
                    new ProbabilityCalculatorFactory()));
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
        [InlineData("(P4-2)(C2)", 2, 0.13889, 0.29622, 0.32166)]
        [InlineData("(P4-2)(C2)", 0, 0.16102)]
        [InlineData("(P4-2)(C1)", 0, 0.16656)]
        [InlineData("(P4-2)(C2:DC)", 0, 0.22141)]
        [InlineData("(P4-2)(C2:DC,C)", 0, 0.28252)]
        [InlineData("(2,3:P4)", 1, 0.55556, 0.84877)]
        [InlineData("(2,3,4,5,6:P4)", 0, 0.03472)]
        [InlineData("(2,3,4,5,6:P4)", 1, 0.01543, 0.07223)]
        [InlineData("(2,3,4,5,6:P4)", 5, 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("1D1:B", 0, 0.00)]
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
