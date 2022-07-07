namespace ActionCalculator.Models.Actions
{
	public class Foul : Action
	{
		public Foul(int roll) : base(ActionType.Foul, roll, false)
		{
		}

		public override bool IsRerollable() => false;
	}
}