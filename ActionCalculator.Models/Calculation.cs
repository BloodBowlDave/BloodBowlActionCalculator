namespace ActionCalculator.Models
{
    public class Calculation
    {
        public Calculation(PlayerActions playerActions, int rerolls, Season season = Season.Season3)
        {
            PlayerActions = playerActions;
            Rerolls = rerolls;
            Season = season;
        }

        public PlayerActions PlayerActions { get; }
        public int Rerolls { get; set; }
        public Season Season { get; set; } = Season.Season3;

        public override string ToString() =>
            PlayerActions.ToString() + (Season == Season.Season2 ? "~S2" : "");
    }
}