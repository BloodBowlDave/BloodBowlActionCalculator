namespace ActionCalculator.Abstractions
{
    public class Action
    {
        public Action(ActionType actionType, decimal success, decimal failure, decimal nonCriticalFailure,
            bool rerollNonCriticalFailure)
        {
            ActionType = actionType;
            Success = success;
            Failure = failure;
            NonCriticalFailure = nonCriticalFailure;
            RerollNonCriticalFailure = rerollNonCriticalFailure;
        }

        public ActionType ActionType { get; }
        public decimal Success { get; }
        public decimal Failure { get; }
        public decimal NonCriticalFailure { get; }
        public bool RerollNonCriticalFailure { get; }
    }
}
