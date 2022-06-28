namespace ActionCalculator.Models
{
    public class CalculationResult
    {
        public CalculationResult(PlayerActions playerActions, List<decimal[]> results)
        {
            PlayerActions = playerActions;
            Results = results;
        }

        public PlayerActions PlayerActions { get; }
        public List<decimal[]> Results { get; }
    }
}