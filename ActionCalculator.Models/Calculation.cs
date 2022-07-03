namespace ActionCalculator.Models
{
    public class Calculation
    {
        public Calculation(PlayerActions playerActions, int rerolls)
        {
            PlayerActions = playerActions;
            Rerolls = rerolls;
            Results = new decimal[17];
        }

        public PlayerActions PlayerActions { get; }
        public int Rerolls { get; set; }
        public decimal[] Results { get; }
    }
}