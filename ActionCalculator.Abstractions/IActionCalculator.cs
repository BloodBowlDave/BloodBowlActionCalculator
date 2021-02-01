namespace ActionCalculator.Abstractions
{
    public interface IActionCalculator
    {
        decimal[] Calculate(Calculation calculation);
    }
}