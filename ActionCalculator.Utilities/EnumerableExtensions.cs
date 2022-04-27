namespace ActionCalculator.Utilities
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Combinations<T>(this IEnumerable<T> source)
        {
            if (null == source)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var data = source.ToArray();

            return Enumerable
                .Range(0, 1 << data.Length)
                .Select(index => data
                    .Where((_, i) => (index & (1 << i)) != 0)
                    .ToArray());
        }
    }
}