namespace ActionCalculator
{
    public class ProbabilityComparer : IEqualityComparer<decimal>
    {
        public bool Equals(decimal x, decimal y) => Math.Round(x, 10) == Math.Round(y, 10);

        public int GetHashCode(decimal obj) => Math.Round(obj, 10).GetHashCode();
    }
}
