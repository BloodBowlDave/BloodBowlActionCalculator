namespace ActionCalculator.Models.Actions
{
	public class Interference : Action
	{
		public Interference(int roll) : base(ActionType.Interference, roll, false)
		{
		}

		public override bool IsRerollable() => false;
	}
}