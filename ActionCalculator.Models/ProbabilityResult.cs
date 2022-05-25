namespace ActionCalculator.Models
{
    public class ProbabilityResult
    {
        public ProbabilityResult(decimal[] probabilities)
        {
            Probabilities = probabilities;
        }

        public decimal[] Probabilities { get; }
    }
}