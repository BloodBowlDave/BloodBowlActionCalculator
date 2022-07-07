namespace ActionCalculator.Models.Actions
{
	public class Bribe : Action
	{
		public Bribe() : base(ActionType.Bribe, 0, false)
		{
		}
    
		public override string ToString() => ((char) ActionType).ToString();

		public override bool IsRerollable() => false;
	}
}