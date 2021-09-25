namespace ActionCalculator.Abstractions
{
    public class Action
    {
        public Action(ActionType actionType, decimal success, decimal failure)
        {
            ActionType = actionType;
            Success = success;
            Failure = failure;
        }

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
    }
}
