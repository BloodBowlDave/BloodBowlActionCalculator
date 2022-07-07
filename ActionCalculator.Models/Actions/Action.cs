namespace ActionCalculator.Models.Actions
{
    public class Action
    {
        protected Action(ActionType actionType, int roll, bool usePro)
        {
            ActionType = actionType;
            UsePro = usePro;
            Roll = roll;
        }

        public ActionType ActionType { get; }
        public bool UsePro { get; }
        public int Roll { get; }
        
        public virtual bool IsRerollable() => true;

        public override string ToString() => $"{(char) ActionType}{Roll}{(UsePro ? "*" : "")}";

        public virtual string GetDescription() => $"{Roll}+ {ActionType}";
    }
}
