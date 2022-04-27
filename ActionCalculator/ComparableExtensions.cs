namespace ActionCalculator
{
    public static class ComparableExtensions
    {
        public static T ThisOrMaximum<T>(this T comparable, T maximum) where T : IComparable => 
            comparable.CompareTo(maximum) > 0 ? maximum : comparable;

        public static T ThisOrMinimum<T>(this T comparable, T minimum) where T : IComparable =>
            comparable.CompareTo(minimum) < 0 ? minimum : comparable;
    }
}
