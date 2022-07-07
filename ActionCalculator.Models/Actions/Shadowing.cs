namespace ActionCalculator.Models.Actions
{
	public class Shadowing : Action
	{
		public Shadowing(bool usePro, bool rerollFailure, int difference) : base(ActionType.Shadowing, difference, usePro)
		{
			RerollFailure = rerollFailure;
			Difference = difference;
		}
    
		public bool RerollFailure { get; }
		private int Difference { get; }

		public override string ToString() => $"{(char) ActionType}{GetDifference()}{(!RerollFailure ? "'" : "")}";

		private string GetDifference() => Difference > 0 ? "+" + Difference : Difference.ToString();
	}
}