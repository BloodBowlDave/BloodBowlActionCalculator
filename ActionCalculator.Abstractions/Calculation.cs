namespace ActionCalculator.Abstractions
{
    public class Calculation
    {
        public Calculation(Player[] players)
        {
            Players = players;
        }

        public Player[] Players { get; }
    }
}