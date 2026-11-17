namespace ActionCalculator.Models.Actions
{
	public class Shadowing(bool usePro, bool rerollFailure, int difference) : Action(ActionType.Shadowing, difference, usePro)
	{
        public bool RerollFailure { get; } = rerollFailure;

        public override string ToString() => $"{(char) ActionType}{GetDifference()}{(!RerollFailure ? "'" : "")}";

		private string GetDifference() => Roll > 0 ? "+" + Roll : Roll.ToString();
	}
}