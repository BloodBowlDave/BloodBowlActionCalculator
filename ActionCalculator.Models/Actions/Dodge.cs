namespace ActionCalculator.Models.Actions
{
	public class Dodge : Action
	{
		public Dodge(int roll, bool usePro, bool useDivingTackle, bool useBreakTackle) : base(ActionType.Dodge, roll, usePro)
		{
			UseDivingTackle = useDivingTackle;
			UseBreakTackle = useBreakTackle;
		}
    
		public bool UseDivingTackle { get; }
		public bool UseBreakTackle { get; }

		public override string ToString() => $"{(char) ActionType}{Numerator}{(!UseBreakTackle ? "¬" : "")}{(UseDivingTackle ? "\"" : "")}{(UsePro ? "*" : "")}";
	}
}