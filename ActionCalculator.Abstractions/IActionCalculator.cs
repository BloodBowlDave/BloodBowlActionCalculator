namespace ActionCalculator.Abstractions
{
    public interface IActionCalculator
    {
        public CalculationResult Calculate(string calculation);
    }
}