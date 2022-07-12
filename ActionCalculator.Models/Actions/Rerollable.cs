namespace ActionCalculator.Models.Actions
{
	public class Rerollable : Action
	{
		public Rerollable(int roll, bool usePro) : base(ActionType.Rerollable, roll, usePro)
		{
		}
    
		public override string ToString() => Numerator + (UsePro ? "*" : "");
	}
}