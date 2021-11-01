using System;
using System.Collections.Generic;

namespace ActionCalculator
{
    public class ProbabilityComparer : IEqualityComparer<decimal>
    {
        public bool Equals(decimal x, decimal y)
        {
            return Math.Round(x, 10) == Math.Round(y, 10);
        }

        public int GetHashCode(decimal obj)
        {
            return Math.Round(obj, 10).GetHashCode();
        }
    }
}
