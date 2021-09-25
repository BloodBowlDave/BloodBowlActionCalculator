using System.Collections.Generic;

namespace ActionCalculator.Abstractions
{
    public interface IProbabilityCalculator
    {
        IEnumerable<decimal> Calculate(Calculation calculation);
    }
}