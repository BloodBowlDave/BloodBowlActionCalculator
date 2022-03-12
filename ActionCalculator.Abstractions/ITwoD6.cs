namespace ActionCalculator.Abstractions
{
    public interface ITwoD6
    {
        decimal Success(int minimumRoll);
        decimal RollDouble(int minimumRoll);
    }
}