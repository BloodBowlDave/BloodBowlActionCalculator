namespace ActionCalculator.Abstractions
{
    public class Action
    {
        public Action(ActionType actionType, decimal success, decimal failure, decimal nonCriticalFailure)
        {
            ActionType = actionType;
            Success = success;
            Failure = failure;
            NonCriticalFailure = nonCriticalFailure;
        }

        public ActionType ActionType { get; }
        public decimal Success { get; }
        public decimal Failure { get; }
        public decimal NonCriticalFailure { get; }
        public bool UseProBeforeReroll { get; set; }
        public bool UseBrawlerBeforeReroll { get; set; }
        public bool RerollNonCriticalFailure { get; set; }
        public bool AffectedByDivingTackle { get; set; }
        public int OriginalRoll { get; set; }
        public int NumberOfDice { get; set; }
        public int NumberOfSuccessfulResults { get; set; }
        public decimal SuccessOnOneDie { get; set; }
        public decimal SuccessOnTwoDice { get; set; }
    }
}
