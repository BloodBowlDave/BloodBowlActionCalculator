namespace ActionCalculator.Models.Actions
{
	public class Rerollable : Action
	{
		public Rerollable(bool usePro, int roll) : base(ActionType.Rerollable, roll, usePro)
		{
		}
    
		public override string ToString() => Roll + (UsePro ? "*" : "");
	}
}