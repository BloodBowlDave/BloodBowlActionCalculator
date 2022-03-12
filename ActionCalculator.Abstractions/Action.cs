namespace ActionCalculator.Abstractions
{
    public class Action
    {
        public Action(ActionType actionType, decimal success, decimal failure, decimal nonCriticalFailure, decimal successOnOneDie)
        {
            ActionType = actionType;
            Success = success;
            Failure = failure;
            NonCriticalFailure = nonCriticalFailure;
            SuccessOnOneDie = successOnOneDie;
        }

        public ActionType ActionType { get; }
        public decimal Success { get; }
        public decimal Failure { get; }
        public decimal NonCriticalFailure { get; }
        public decimal SuccessOnOneDie { get; }
        public bool UsePro { get; set; }
        public bool UseBrawler { get; set; }
        public bool RerollNonCriticalFailure { get; set; }
        public bool AffectedByDivingTackle { get; set; }
        public int OriginalRoll { get; set; }
        public int NumberOfDice { get; set; }
        public int NumberOfSuccessfulResults { get; set; }
        public decimal SuccessOnTwoDice { get; set; }
        public bool RequiresDauntlessFail { get; set; }
        public bool RequiresRemoval { get; set; }
    }
}
