namespace ActionCalculator.Models.Actions
{
	public class Dauntless(int roll, bool rerollFailure, bool usePro) : Action(ActionType.Dauntless, roll, usePro)
	{
        public bool RerollFailure { get; set; } = rerollFailure;

        public override string ToString() => $"{(char) ActionType}{Roll}{(!RerollFailure ? "'" : "")}{(UsePro ? "*" : "")}";
	}
}