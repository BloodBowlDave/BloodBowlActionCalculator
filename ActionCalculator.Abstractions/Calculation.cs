namespace ActionCalculator.Abstractions
{
    public class Calculation
    {
        public Calculation(PlayerAction[] playerActions)
        {
            PlayerActions = playerActions;
        }

        public PlayerAction[] PlayerActions { get; }
    }
}