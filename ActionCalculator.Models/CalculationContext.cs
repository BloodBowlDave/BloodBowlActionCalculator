namespace ActionCalculator.Models
{
    public class CalculationContext
    {
        public CalculationContext(Calculation calculation, int maxRerolls, decimal[] results, decimal[] sendOffResults)
        {
            Calculation = calculation;
            MaxRerolls = maxRerolls;
            Results = results;
            SendOffResults = sendOffResults;
        }

        public Calculation Calculation { get; }
        public int MaxRerolls { get; }
        public decimal[] Results { get; }
        public decimal[] SendOffResults { get; }
    }
}