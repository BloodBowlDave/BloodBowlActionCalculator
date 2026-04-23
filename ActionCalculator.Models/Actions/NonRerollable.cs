using ActionCalculator.Utilities;

namespace ActionCalculator.Models.Actions
{
	public class NonRerollable(int numerator, int denominator) : Action(ActionType.NonRerollable, numerator, false)
	{
        public int Denominator { get; } = denominator;

        public override string ToString() => $"{Roll}/{Denominator}";

        public override string GetDescription() => $"{Roll}/{Denominator} {ActionType.ToString().PascalCaseToSpaced()}";

        public override bool IsRerollable() => false;
	}
}