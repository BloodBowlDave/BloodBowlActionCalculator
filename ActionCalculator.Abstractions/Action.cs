namespace ActionCalculator.Abstractions
{
    public class Action
    {
        private readonly string _action;

        public Action(decimal success, decimal failure, string action)
        {
            Success = success;
            Failure = failure;
            _action = action;
        }
        
        public decimal Success { get; }
        public decimal Failure { get; }

        public override string ToString()
        {
            return _action;
        }
    }
}
