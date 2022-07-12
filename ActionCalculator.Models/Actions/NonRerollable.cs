namespace ActionCalculator.Models.Actions
{
	public class NonRerollable : Action
	{
		public NonRerollable(int numerator, int denominator) : base(ActionType.NonRerollable, numerator, false)
		{
			Denominator = denominator;
		}
    
		public int Denominator { get; }

		public override string ToString() => $"{(char) ActionType}{Numerator}/{Denominator}";

		public override bool IsRerollable() => false;
	}
}