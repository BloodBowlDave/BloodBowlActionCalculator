using ActionCalculator.Abstractions;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class CalculatorForAllRerolls : ICalculatorForAllRerolls
    {
        private readonly IPlayerActionsBuilder _playerActionsBuilder;
        private readonly IEqualityComparer<decimal> _probabilityComparer;
        private readonly ICalculator _calculator;

        private const int MaximumRerolls = 8;

        public CalculatorForAllRerolls(
            IPlayerActionsBuilder playerActionsBuilder,
            IEqualityComparer<decimal> probabilityComparer,
            ICalculator calculator)
        {
            _playerActionsBuilder = playerActionsBuilder;
            _probabilityComparer = probabilityComparer;
            _calculator = calculator;
        }

        public IEnumerable<CalculationResult> CalculateForAllRerolls(string playerActionsString)
        {
            var playerActions = _playerActionsBuilder.Build(playerActionsString);

            var probabilityResults = new List<CalculationResult>();

            for (var r = 0; r < MaximumRerolls; r++)
            {
                var calculation = new Calculation(playerActions, r);

                var result = _calculator.Calculate(calculation);
                
                if (r > 0 && probabilityResults[r - 1].Results.SequenceEqual(result.Results, _probabilityComparer))
                {
                    break;
                }

                probabilityResults.Add(result);

                yield return result;
            }
        }
    }
}