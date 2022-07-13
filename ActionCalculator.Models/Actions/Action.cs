using ActionCalculator.Utilities;

namespace ActionCalculator.Models.Actions
{
    public class Action
    {
        protected Action(ActionType actionType, int roll, bool usePro)
        {
            ActionType = actionType;
            UsePro = usePro;
            Numerator = roll;
        }

        public ActionType ActionType { get; }
        public bool UsePro { get; set; }
        public int Numerator { get; }
        
        public virtual bool IsRerollable() => true;

        public override string ToString() => $"{(char) ActionType}{Numerator}{(UsePro ? "*" : "")}";

        public virtual string GetDescription() => $"{Numerator}+ {ActionType.ToString().PascalCaseToSpaced()}";
    }
}
