﻿using ActionCalculator.Abstractions;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class ActionCalculator : IActionCalculator
    {
        private readonly ICalculationBuilder _calculationBuilder;
        private readonly IEqualityComparer<decimal> _probabilityComparer;
        private readonly IActionMediator _actionMediator;

        private const int MaximumRerolls = 8;

        public ActionCalculator(
            ICalculationBuilder calculationBuilder,
            IEqualityComparer<decimal> probabilityComparer,
            IActionMediator actionMediator)
        {
            _calculationBuilder = calculationBuilder;
            _probabilityComparer = probabilityComparer;
            _actionMediator = actionMediator;
        }

        public CalculationResult Calculate(string calculationString)
        {
            var calculation = _calculationBuilder.Build(calculationString);

            var probabilityResults = new List<decimal[]>();
            var sendOffResults = new List<decimal[]>();

            for (var r = 0; r < MaximumRerolls; r++)
            {
                var context = new CalculationContext(calculation, r, 
                    new decimal[calculation.PlayerActions.Length * 2 + 1], 
                    new decimal[calculation.PlayerActions.Length * 2 + 1]);

                _actionMediator.Initialise(context);
                _actionMediator.Resolve(1m, r, -1, Skills.None);

                var results = context.Results.Where(x => x > 0).ToArray();

                if (r > 0 && probabilityResults[r - 1].SequenceEqual(results, _probabilityComparer))
                {
                    break;
                }

                probabilityResults.Add(results);

                sendOffResults.Add(context.SendOffResults.Where(x => x > 0).ToArray());
            }

            foreach (var probabilityResult in probabilityResults)
            {
                AggregateResults(probabilityResult);
            }

            foreach (var sendOffResult in sendOffResults)
            {
                AggregateResults(sendOffResult);
            }

            return new CalculationResult(calculation, probabilityResults, sendOffResults);
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