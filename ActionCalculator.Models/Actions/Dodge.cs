namespace ActionCalculator.Models.Actions
{
	public class Dodge : Action
	{
		public Dodge(decimal success, decimal failure, int roll, bool usePro, bool useDivingTackle, bool useBreakTackle) 
			: base(ActionType.Dodge, success, failure, roll, usePro)
		{
			UseDivingTackle = useDivingTackle;
			UseBreakTackle = useBreakTackle;
		}
    
		public bool UseDivingTackle { get; }
		public bool UseBreakTackle { get; }

		public override string ToString() => $"{(char) ActionType}{Roll}{(!UseBreakTackle ? "¬" : "")}{(UseDivingTackle ? "\"" : "")}{(UsePro ? "*" : "")}";
	}
}