namespace ActionCalculator.Models.Actions
{
	public class NonRerollable : Action
	{
		public NonRerollable(int roll, int denominator) : base(ActionType.NonRerollable, roll, false)
		{
			Denominator = denominator;
		}
    
		public int Denominator { get; }

		public override string ToString() => $"{(char) ActionType}{Roll}/{Denominator}";

		public override bool IsRerollable() => false;
	}
}