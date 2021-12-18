namespace ActionCalculator.Abstractions
{
    public class CalculationContext
    {
        public CalculationContext(Calculation calculation, int maxRerolls, decimal[] results)
        {
            Calculation = calculation;
            MaxRerolls = maxRerolls;
            Results = results;
        }

        public Calculation Calculation { get; }
        public int MaxRerolls { get; }
        public decimal[] Results { get; }
    }
}