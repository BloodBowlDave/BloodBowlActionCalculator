namespace ActionCalculator.Models.Actions
{
	public class Injury : Action
	{
		public Injury(int roll) : base(ActionType.Injury, roll, false)
		{
		}

		public override bool IsRerollable() => false;
	}
}