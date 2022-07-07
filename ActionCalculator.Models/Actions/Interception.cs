namespace ActionCalculator.Models.Actions
{
	public class Interception : Action
	{
		public Interception(int roll) : base(ActionType.Interception, roll, false)
		{
		}

		public override bool IsRerollable() => false;
	}
}