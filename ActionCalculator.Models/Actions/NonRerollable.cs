using ActionCalculator.Utilities;

namespace ActionCalculator.Models.Actions
{
	public class NonRerollable : Action
	{
		public NonRerollable(int numerator, int denominator) : base(ActionType.NonRerollable, numerator, false)
		{
			Denominator = denominator;
		}
    
		public int Denominator { get; }

		public override string ToString() => $"{Roll}/{Denominator}";

        public override string GetDescription() => $"{Roll}/{Denominator} {ActionType.ToString().PascalCaseToSpaced()}";

        public override bool IsRerollable() => false;
	}
}