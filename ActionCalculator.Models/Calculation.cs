namespace ActionCalculator.Models
{
    public class Calculation
    {
        public Calculation(PlayerActions playerActions, int rerolls)
        {
            PlayerActions = playerActions;
            Rerolls = rerolls;
        }

        public PlayerActions PlayerActions { get; }
        public int Rerolls { get; set; }
    }
}