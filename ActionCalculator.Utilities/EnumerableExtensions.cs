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

            return Enumerable.Range(0, 1 << data.Length)
                .Select(index => data
                    .Where((_, i) => (index & (1 << i)) != 0)
                    .ToArray());
        }

        public static IEnumerable<List<T>> GetCombinationsOfLists<T>(this IEnumerable<List<T>> lists)
        {
            IEnumerable<List<T>> combinations = new List<List<T>> { new() };

            return lists.Aggregate(combinations, (current, inner)
                => current.SelectMany(x => inner.Select(value => new List<T>(x) { value })
                    .ToList()));
        }
    }
}