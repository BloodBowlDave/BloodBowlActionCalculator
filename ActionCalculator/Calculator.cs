using ActionCalculator.Abstractions;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class Calculator : ICalculator
    {
        private readonly IPlayerActionsBuilder _playerActionsBuilder;
        private readonly IEqualityComparer<decimal> _probabilityComparer;
        private readonly IActionMediator _actionMediator;

        private const int MaximumRerolls = 8;

        public Calculator(
            IPlayerActionsBuilder playerActionsBuilder,
            IEqualityComparer<decimal> probabilityComparer,
            IActionMediator actionMediator)
        {
            _playerActionsBuilder = playerActionsBuilder;
            _probabilityComparer = probabilityComparer;
            _actionMediator = actionMediator;
        }

        public CalculationResult Calculate(string playerActionsString)
        {
            var playerActions = _playerActionsBuilder.Build(playerActionsString);

            var probabilityResults = new List<decimal[]>();

            for (var r = 0; r < MaximumRerolls; r++)
            {
                var context = new Calculation(playerActions, r);

                _actionMediator.Initialise(context);
                _actionMediator.Resolve(1m, r, -1, Skills.None);

                var results = context.Results.Where(x => x > 0).ToArray();

                if (r > 0 && probabilityResults[r - 1].SequenceEqual(results, _probabilityComparer))
                {
                    break;
                }

                probabilityResults.Add(results);
            }

            foreach (var probabilityResult in probabilityResults)
            {
                AggregateResults(probabilityResult);
            }

            return new CalculationResult(playerActions, probabilityResults);
        }

        private static void AggregateResults(IList<decimal> result)
        {
            for (var i = 1; i < result.Count; i++)
            {
                result[i] += result[i - 1];
            }
        }
    }
}