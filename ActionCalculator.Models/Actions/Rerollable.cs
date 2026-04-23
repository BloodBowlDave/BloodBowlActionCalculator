namespace ActionCalculator.Models.Actions
{
	public class Rerollable(int roll, bool usePro) : Action(ActionType.Rerollable, roll, usePro)
	{
        public override string ToString() => Roll + (UsePro ? "*" : "");
	}
}