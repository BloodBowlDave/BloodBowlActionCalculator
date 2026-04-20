namespace ActionCalculator.Models.Actions
{
    public class Leap : Action
    {
        public Leap(int roll, bool usePro, bool useDivingTackle = false) : base(ActionType.Leap, roll, usePro)
        {
            UseDivingTackle = useDivingTackle;
        }

        public bool UseDivingTackle { get; set; }

        public override string ToString() => $"{(char)ActionType}{Roll}{(UseDivingTackle ? "\"" : "")}{(UsePro ? "*" : "")}";
    }
}
