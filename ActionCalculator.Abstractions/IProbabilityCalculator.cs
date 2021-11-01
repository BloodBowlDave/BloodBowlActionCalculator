using System.Collections.Generic;

namespace ActionCalculator.Abstractions
{
    public interface IProbabilityCalculator
    {
        IReadOnlyList<ProbabilityResult> Calculate(Calculation calculation);
    }
}