namespace ActionCalculator.Models.Actions
{
    public class Action
    {
        protected Action(ActionType actionType, decimal success, decimal failure, int roll, bool usePro)
        {
            ActionType = actionType;
            Success = success;
            Failure = failure;
            UsePro = usePro;
            Roll = roll;
        }

        public ActionType ActionType { get; }
        public decimal Success { get; }
        public decimal Failure { get; }
        public bool UsePro { get; }
        public int Roll { get; }

        public override string ToString() => $"{(char) ActionType}{Roll}{(UsePro ? "*" : "")}";
    }
}
