namespace ActionCalculator.Models.Actions
{
	public class Tentacles : Action
	{
		public Tentacles(bool usePro, bool rerollFailure, int difference) : base(ActionType.Tentacles, difference, usePro)
		{
			RerollFailure = rerollFailure;
		}
    
		public bool RerollFailure { get; }

		public override string ToString() => $"{(char)ActionType}{GetModifier()}{(!RerollFailure ? "'" : "")}";

		private string GetModifier() => Numerator > 0 ? "+" + Numerator : Numerator.ToString();
	}
}