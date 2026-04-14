using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ActionCalculator.Tests
{
    public class ActionCalculatorPassS2Tests
    {
        private readonly ICalculator _calculator;
        private readonly IPlayerActionsBuilder _playerActionsBuilder;

        public ActionCalculatorPassS2Tests()
        {
            var services = new ServiceCollection();

            services.AddActionCalculatorServices();

            var serviceProvider = services.BuildServiceProvider();

            _calculator = serviceProvider.GetService<ICalculator>();
            _playerActionsBuilder = serviceProvider.GetService<IPlayerActionsBuilder>();
        }

        [Theory]
        [InlineData("P4-2,I6;C2", 0, 0.13050)]
        [InlineData("P4-2,I7;C2", 0, 0.13419)]
        [InlineData("P4-2;C,DC:C2", 0, 0.28252)]
        [InlineData("P4-2;C1", 0, 0.16656)]
        [InlineData("P4-2;C2", 0, 0.16102)]
        [InlineData("P4-2;C2", 2, 0.13889, 0.29622, 0.32166)]
        [InlineData("P4-2;DC:C2", 0, 0.22141)]
        [InlineData("PA:P2,I6;C2", 1, 0.67515, 0.78768)]
        [InlineData("PA:P2;C2", 1, 0.81019, 0.94522)]
        [InlineData("CB,PA:P2,I6;C2", 1, 0.78768, 0.91896)]
        public void ActionCalculatorReturnsExpectedResultForSeason2Pass(string playerActionsString, int rerolls, params double[] expected)
        {
            var playerActions = _playerActionsBuilder.Build(playerActionsString);
            var calculation = new Calculation(playerActions, rerolls, "Season 2");
            var result = _calculator.Calculate(calculation);

            Assert.Equal(expected.Length, result.Results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal((decimal)expected[i], result.Results[i], 5);
            }

            Assert.Equal(playerActionsString + "~Season 2", calculation.ToString());
        }
    }
}
