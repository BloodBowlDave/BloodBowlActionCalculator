namespace ActionCalculator.Models
{
    public class CalculationResult
    {
        public CalculationResult(Calculation calculation, List<ProbabilityResult> probabilityResults)
        {
            Calculation = calculation;
            ProbabilityResults = probabilityResults;
        }

        public Calculation Calculation { get; }
        public List<ProbabilityResult> ProbabilityResults { get; }
    }
}