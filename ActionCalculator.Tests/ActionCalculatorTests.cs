using ActionCalculator.Abstractions;
using Xunit;

namespace ActionCalculator.Tests
{
    public class ActionCalculatorTests
    {
        private readonly ActionCalculator _actionCalculator;
        private readonly ActionBuilder _actionBuilder;
        private readonly PlayerParser _playerParser;
        private readonly CalculationBuilder _calculationBuilder;

        public ActionCalculatorTests()
        {
            _actionCalculator = new ActionCalculator();
            _actionBuilder = new ActionBuilder();
            _playerParser = new PlayerParser();
            _calculationBuilder = new CalculationBuilder(_actionBuilder, _playerParser);
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
        public void ActionCalculatorReturnsExpectedResult(string calculation, params double[] expected)
        {
            var result = _actionCalculator.Calculate(_calculationBuilder.Build(calculation));

            Assert.Equal(expected.Length, result.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal((decimal) expected[i], result[i], 5);
            }
        }
    }
}
