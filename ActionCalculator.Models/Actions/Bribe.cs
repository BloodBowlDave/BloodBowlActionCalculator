namespace ActionCalculator.Models.Actions
{
	public class Bribe : Action
	{
		public Bribe(decimal success, decimal failure) 
			: base(ActionType.Bribe, success, failure, 0, false)
		{
		}
    
		public override string ToString() => ((char) ActionType).ToString();

		public override bool IsRerollable() => false;
	}
}