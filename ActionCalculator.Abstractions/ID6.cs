namespace ActionCalculator.Abstractions
{
    public interface ID6
    {
        IEnumerable<int> Rolls();
        List<List<int>> Rolls(int numberOfDice);
        decimal Success(int numberOfDice, int minimumRoll);
        decimal RollDouble(int minimumRoll);
    }
}