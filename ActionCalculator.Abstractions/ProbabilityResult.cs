using System.Collections.Generic;

namespace ActionCalculator.Abstractions
{
    public class ProbabilityResult
    {
        public ProbabilityResult(decimal[] probabilities)
        {
            Probabilities = probabilities;
        }

        public decimal[] Probabilities { get; }
    }
}