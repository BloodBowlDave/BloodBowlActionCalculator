//using ActionCalculator.Abstractions;
//using Xunit;

//namespace ActionCalculator.Tests
//{
//    public class DiceRollingActionCalculatorTests
//    {
//        private readonly DiceRollingActionCalculator _actionCalculator;

//        public DiceRollingActionCalculatorTests()
//        {
//            _actionCalculator = new DiceRollingActionCalculator();
//        }

//        [Fact]
//        public void TwoPlusTwoPlusWithReroll()
//        {
//            var calculation = new Calculation("2,2:1");

//            var result = _actionCalculator.Calculate(calculation);

//            Assert.Equal(0.92593m, result.P, 5);
//        }
//    }
//}
