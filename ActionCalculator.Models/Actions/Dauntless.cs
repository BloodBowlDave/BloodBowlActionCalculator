namespace ActionCalculator.Models.Actions
{
	public class Dauntless : Action
	{
		public Dauntless(int roll, bool rerollFailure, bool usePro) : base(ActionType.Dauntless, roll, usePro)
		{
			RerollFailure = rerollFailure;
		}
    
		public bool RerollFailure { get; set; }

        public override string ToString() => $"{(char) ActionType}{Roll}{(!RerollFailure ? "'" : "")}{(UsePro ? "*" : "")}";
	}
}