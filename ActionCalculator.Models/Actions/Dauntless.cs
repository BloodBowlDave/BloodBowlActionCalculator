namespace ActionCalculator.Models.Actions
{
	public class Dauntless : Action
	{
		public Dauntless(bool rerollFailure, int roll, bool usePro) : base(ActionType.Dauntless, roll, usePro)
		{
			RerollFailure = rerollFailure;
		}
    
		public bool RerollFailure { get; }

		public override string ToString() => $"{(char) ActionType}{Roll}{(!RerollFailure ? "'" : "")}{(UsePro ? "*" : "")}";
	}
}