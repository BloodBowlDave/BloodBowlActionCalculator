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
        public int NumberOfSuccessfulResults { get; }
        public int NumberOfNonCriticalFailures { get; }
        public bool RerollNonCriticalFailure { get; }

        public override string ToString() => $"{NumberOfDice}D{NumberOfSuccessfulResults}{(NumberOfNonCriticalFailures > 0 ? "!" + NumberOfNonCriticalFailures : "")}{(UseBrawler ? "^" : "")}{(UsePro ? "*" : "")}{(!RerollNonCriticalFailure ? "'" : "")}";

        public override string GetDescription()
        {
	        var successString = NumberOfSuccessfulResults > 1
		        ? $"{NumberOfSuccessfulResults} Successes"
		        : "1 Success";

	        return NumberOfDice switch
	        {
		        3 => $"3 Dice Block {successString}",
		        2 => $"2 Dice Block {successString}",
		        1 => $"1 Die Block {successString}",
		        -2 => $"2 Red Dice Block {successString}",
		        -3 => $"3 Red Dice Block {successString}",
		        _ => throw new ArgumentOutOfRangeException()
	        };
        }
    }
}
