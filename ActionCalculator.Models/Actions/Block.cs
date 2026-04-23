namespace ActionCalculator.Models.Actions
{
    public class Block(int numberOfDice, int numberOfSuccessfulResults, int numberOfNonCriticalFailures,
        bool useBrawler, bool usePro, bool rerollNonCriticalFailure, bool useHatred = false) : Action(ActionType.Block, 0, usePro)
    {
        public bool UseBrawler { get; set; } = useBrawler;
        public bool UseHatred { get; set; } = useHatred;
        public int NumberOfDice { get; set; } = numberOfDice;
        public int NumberOfSuccessfulResults { get; set; } = numberOfSuccessfulResults;
        public int NumberOfNonCriticalFailures { get; set; } = numberOfNonCriticalFailures;
        public bool RerollNonCriticalFailure { get; } = rerollNonCriticalFailure;

        public override string ToString() => $"{NumberOfDice}D{NumberOfSuccessfulResults}{(NumberOfNonCriticalFailures > 0 ? "!" + NumberOfNonCriticalFailures : "")}{(UseBrawler ? "^" : "")}{(UseHatred ? "%" : "")}{(UsePro ? "*" : "")}{(!RerollNonCriticalFailure ? "'" : "")}";

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
