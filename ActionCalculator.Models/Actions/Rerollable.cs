namespace ActionCalculator.Models.Actions
{
	public class Rerollable : Action
	{
		public Rerollable(decimal success, decimal failure, bool usePro, int roll) 
			: base(ActionType.Rerollable, success, failure, roll, usePro)
		{
		}
    
		public override string ToString() => Roll + (UsePro ? "*" : "");
	}
}