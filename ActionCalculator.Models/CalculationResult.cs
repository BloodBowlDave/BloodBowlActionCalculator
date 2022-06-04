namespace ActionCalculator.Models
{
    public class CalculationResult
    {
        public CalculationResult(Calculation calculation, List<decimal[]> results, List<decimal[]> sendOffResults)
        {
            Calculation = calculation;
            Results = results;
            SendOffResults = sendOffResults;
        }

        public Calculation Calculation { get; }
        public List<decimal[]> Results { get; }
        public List<decimal[]> SendOffResults { get; }
    }
}