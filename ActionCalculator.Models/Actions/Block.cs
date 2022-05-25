namespace ActionCalculator.Models.Actions
{
    public class Block : Action
    {
        public Block(int numberOfDice, int numberOfSuccessfulResults, int numberOfNonCriticalFailures, 
            bool useBrawler, bool usePro, bool rerollNonCriticalFailure) 
            : base(ActionType.Block, 0, 0, 0, usePro)
        {
            NumberOfDice = numberOfDice;
            NumberOfSuccessfulResults = numberOfSuccessfulResults;
            NumberOfNonCriticalFailures = numberOfNonCriticalFailures;
            UseBrawler = useBrawler;
            RerollNonCriticalFailure = rerollNonCriticalFailure;
        }

        public bool UseBrawler { get; }
        public int NumberOfDice { get; }
        public int NumberOfSuccessfulResults { get; }
        public int NumberOfNonCriticalFailures { get; }
        public bool RerollNonCriticalFailure { get; }

        public override string ToString() => $"{NumberOfDice}D{NumberOfSuccessfulResults}{(NumberOfNonCriticalFailures > 0 ? "!" + NumberOfNonCriticalFailures : "")}{(UseBrawler ? "^" : "")}{(UsePro ? "*" : "")}{(!RerollNonCriticalFailure ? "'" : "")}";
    }
}
