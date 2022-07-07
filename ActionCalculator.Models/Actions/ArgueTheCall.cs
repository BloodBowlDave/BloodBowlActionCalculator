namespace ActionCalculator.Models.Actions
{
	public class ArgueTheCall : Action
	{
		public ArgueTheCall(int roll) : base(ActionType.ArgueTheCall, roll, false)
		{
		}
		
		public override bool IsRerollable() => false;
	}
}