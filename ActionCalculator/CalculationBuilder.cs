using ActionCalculator.Abstractions;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class CalculationBuilder(IPlayerActionsBuilder playerActionsBuilder) : ICalculationBuilder
    {
        public Calculation Build(string calculationString, int rerolls)
        {
            var season = Season.Season3;
            var input = calculationString;

            if (input.EndsWith("~S2"))
            {
                input = input[..^3];
                season = Season.Season2;
            }

            return new Calculation(playerActionsBuilder.Build(input), rerolls, season);
        }
    }
}
