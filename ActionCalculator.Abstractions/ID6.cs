namespace ActionCalculator.Abstractions
{
    public interface ID6
    {
        List<List<int>> Rolls(int numberOfDice);
        decimal Success(int numberOfDice, int minimumRoll);
        decimal RollDouble(int numberOfDice, int minimumRoll);
    }
}