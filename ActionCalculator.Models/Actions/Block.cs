namespace ActionCalculator.Models.Actions
{
    public class Block : Action
    {
        public Block(int numberOfDice, int numberOfSuccessfulResults, int numberOfNonCriticalFailures, 
            bool useBrawler, bool usePro, bool rerollNonCriticalFailure) 
            : base(ActionType.Block, 0, usePro)
        {
            NumberOfDice = numberOfDice;
            NumberOfSuccessfulResults = numberOfSuccessfulResults;
            NumberOfNonCriticalFailures = numberOfNonCriticalFailures;
            UseBrawler = useBrawler;
            RerollNonCriticalFailure = rerollNonCriticalFailure;
        }

        public bool UseBrawler { get; set; }
        public int NumberOfDice { get; }
        public int NumberOfSuccessfulResults { get; set; }
        public int NumberOfNonCriticalFailures { get; }
        public bool RerollNonCriticalFailure { get; }

        public override string ToString() => $"{NumberOfDice}D{NumberOfSuccessfulResults}{(NumberOfNonCriticalFailures > 0 ? "!" + NumberOfNonCriticalFailures : "")}{(UseBrawler ? "^" : "")}{(UsePro ? "*" : "")}{(!RerollNonCriticalFailure ? "'" : "")}";

        public override string GetDescription() =>
            NumberOfDice switch
            {
                3 => "Three Dice Block",
                2 => "Two Dice Block",
                1 => "One Die Block",
                -2 => "Half Dice Block",
                -3 => "Third Dice Block",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
