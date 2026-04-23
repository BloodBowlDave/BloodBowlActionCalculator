namespace ActionCalculator.Models.Actions
{
	public class Dodge(int roll, bool usePro, bool useDivingTackle, bool useBreakTackle) : Action(ActionType.Dodge, roll, usePro)
	{
        public bool UseDivingTackle { get; set; } = useDivingTackle;
        public bool UseBreakTackle { get; set; } = useBreakTackle;

        public override string ToString() => $"{(char) ActionType}{Roll}{(!UseBreakTackle ? "¬" : "")}{(UseDivingTackle ? "\"" : "")}{(UsePro ? "*" : "")}";
	}
}