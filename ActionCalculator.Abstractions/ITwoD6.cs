namespace ActionCalculator.Abstractions
{
    public interface ITwoD6
    {
        decimal Success(int minimumRoll);
        decimal RollDouble(int minimumRoll);
        IEnumerable<Tuple<int, int>> GetCombinationsForRoll(int roll);
    }
}