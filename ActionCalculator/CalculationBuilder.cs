using ActionCalculator.Abstractions;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class CalculationBuilder : ICalculationBuilder
    {
        private readonly IPlayerActionsBuilder _playerActionsBuilder;

        public CalculationBuilder(IPlayerActionsBuilder playerActionsBuilder)
        {
            _playerActionsBuilder = playerActionsBuilder;
        }

        public Calculation Build(string calculationString, int rerolls)
        {
            var season = Season.Season3;
            var input = calculationString;

            if (input.EndsWith("~S2"))
            {
                input = input[..^3];
                season = Season.Season2;
            }

            return new Calculation(_playerActionsBuilder.Build(input), rerolls, season);
        }
    }
}
