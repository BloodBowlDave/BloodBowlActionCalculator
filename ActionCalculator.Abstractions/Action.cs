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
        
        public ActionType ActionType { get; }
        public decimal Success { get; }
        public decimal Failure { get; }
    }
}
