namespace ActionCalculator.Abstractions
{
    public class Player
    {
        public Action[] Actions { get; set; }
        public Skills Skills { get; set; }
        public double? LonerSuccess { get; set; }
        public double? ProSuccess { get; set; }
    }
}
