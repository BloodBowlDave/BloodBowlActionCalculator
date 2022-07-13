namespace ActionCalculator.Models.Actions
{
	public class Dodge : Action
	{
		public Dodge(int roll, bool usePro, bool useDivingTackle, bool useBreakTackle) : base(ActionType.Dodge, roll, usePro)
		{
			UseDivingTackle = useDivingTackle;
			UseBreakTackle = useBreakTackle;
		}
    
		public bool UseDivingTackle { get; set; }
        public bool UseBreakTackle { get; set; }

        public override string ToString() => $"{(char) ActionType}{Numerator}{(!UseBreakTackle ? "¬" : "")}{(UseDivingTackle ? "\"" : "")}{(UsePro ? "*" : "")}";
	}
}