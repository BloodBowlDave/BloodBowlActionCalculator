namespace ActionCalculator.Models.Actions
{
	public class Tentacles : Action
	{
		public Tentacles(decimal success, decimal failure, bool usePro, bool rerollFailure, int difference) 
			: base(ActionType.Tentacles, success, failure, difference, usePro)
		{
			RerollFailure = rerollFailure;
		}
    
		public bool RerollFailure { get; }

		public override string ToString() => $"{(char)ActionType}{GetModifier()}{(!RerollFailure ? "'" : "")}";

		private string GetModifier() => Roll > 0 ? "+" + Roll : Roll.ToString();
	}
}