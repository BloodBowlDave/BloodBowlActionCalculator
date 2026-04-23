namespace ActionCalculator.Models
{
    public class Calculation(PlayerActions playerActions, int rerolls, Season season = Season.Season3)
    {
        public PlayerActions PlayerActions { get; } = playerActions;
        public int Rerolls { get; set; } = rerolls;
        public Season Season { get; set; } = season;

        public override string ToString() =>
            PlayerActions.ToString() + (Season == Season.Season2 ? "~S2" : "");
    }
}