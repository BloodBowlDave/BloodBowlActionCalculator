namespace ActionCalculator.Models
{
    public class CalculationContext
    {
        public CalculationContext(PlayerActions playerActions, int maxRerolls, decimal[] results, decimal[] sendOffResults)
        {
            PlayerActions = playerActions;
            MaxRerolls = maxRerolls;
            Results = results;
            SendOffResults = sendOffResults;
        }

        public PlayerActions PlayerActions { get; }
        public int MaxRerolls { get; }
        public decimal[] Results { get; }
        public decimal[] SendOffResults { get; }
    }
}