namespace ActionCalculator.Models.Actions
{
	public class Tentacles(bool usePro, bool rerollFailure, int difference) : Action(ActionType.Tentacles, difference, usePro)
	{
        public bool RerollFailure { get; } = rerollFailure;

        public override string ToString() => $"{(char)ActionType}{GetModifier()}{(!RerollFailure ? "'" : "")}";

		private string GetModifier() => Roll > 0 ? "+" + Roll : Roll.ToString();
	}
}