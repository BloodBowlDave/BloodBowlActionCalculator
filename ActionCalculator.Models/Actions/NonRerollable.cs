namespace ActionCalculator.Models.Actions
{
	public class NonRerollable : Action
	{
		public NonRerollable(decimal success, decimal failure, int roll, int denominator) 
			: base(ActionType.NonRerollable, success, failure, roll, false)
		{
			Denominator = denominator;
		}
    
		public int Denominator { get; }

		public override string ToString() => $"{(char) ActionType}{Roll}{(Denominator > 0 && Denominator != 6 ? "/" + Denominator : "")}";

		public override bool IsRerollable() => false;
	}
}