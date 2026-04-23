namespace ActionCalculator.Models.Actions
{
    public class Leap(int roll, bool usePro, bool useDivingTackle = false) : Action(ActionType.Leap, roll, usePro)
    {
        public bool UseDivingTackle { get; set; } = useDivingTackle;

        public override string ToString() => $"{(char)ActionType}{Roll}{(UseDivingTackle ? "\"" : "")}{(UsePro ? "*" : "")}";
    }
}
