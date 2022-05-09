namespace ActionCalculator.Abstractions
{
    public interface ITwoD6
    {
        IEnumerable<List<int>> Rolls();
        decimal Success(int minimumRoll);
        decimal RollDouble(int minimumRoll);
    }
}