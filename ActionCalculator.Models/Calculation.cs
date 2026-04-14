namespace ActionCalculator.Models
{
    public class Calculation
    {
        public Calculation(PlayerActions playerActions, int rerolls, string season = "Season 3")
        {
            PlayerActions = playerActions;
            Rerolls = rerolls;
            Season = season;
        }

        public PlayerActions PlayerActions { get; }
        public int Rerolls { get; set; }
        public string Season { get; set; } = "Season 3";

        public override string ToString() =>
            PlayerActions.ToString() + (Season == "Season 2" ? "~Season 2" : "");
    }
}