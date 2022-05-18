namespace ActionCalculator.Abstractions.Calculators.Blocking
{
    public class RollOutcomes
    {
        public int Successes { get; set; }
        public int BrawlerRolls { get; set; }
        public int ProRolls { get; set; }
        public int Failures { get; set; }
        public int BrawlerAndProRolls { get; set; }
        public int NonCriticalFailures { get; set; }
        public int ProNonCriticalFailures { get; set; }
        public int ProMultipleNonCriticalFailures { get; set; }
    }

}
