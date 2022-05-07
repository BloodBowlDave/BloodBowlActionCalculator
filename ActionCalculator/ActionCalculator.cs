using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class ActionCalculator : IActionCalculator
    {
        private readonly ICalculationBuilder _calculationBuilder;
        private readonly IEqualityComparer<decimal> _probabilityComparer;
        private readonly IMasterCalculator _masterCalculator;

        private const int MaximumRerolls = 8;

        public ActionCalculator(
            ICalculationBuilder calculationBuilder,
            IEqualityComparer<decimal> probabilityComparer,
            IMasterCalculator masterCalculator)
        {
            _calculationBuilder = calculationBuilder;
            _probabilityComparer = probabilityComparer;
            _masterCalculator = masterCalculator;
        }

        public CalculationResult Calculate(string calculationString)
        {
            var calculation = _calculationBuilder.Build(calculationString);

            var probabilityResults = new List<ProbabilityResult>();

            for (var rerolls = 0; rerolls < MaximumRerolls; rerolls++)
            {
                var context = new CalculationContext(calculation, rerolls,
                    new decimal[calculation.PlayerActions.Length * 2 + 1]);

                _masterCalculator.Initialise(context);
                _masterCalculator.Calculate(1m, rerolls, null!, Skills.None);

                var results = context.Results.Where(x => x > 0).ToArray();

                if (rerolls > 0 && probabilityResults[rerolls - 1].Probabilities.SequenceEqual(results, _probabilityComparer))
                {
                    break;
                }

                probabilityResults.Add(new ProbabilityResult(results));
            }

            foreach (var probabilityResult in probabilityResults)
            {
                AggregateResults(probabilityResult.Probabilities);
            }

            return new CalculationResult(calculation, probabilityResults.ToList());
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