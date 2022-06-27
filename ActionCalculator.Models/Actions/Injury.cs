namespace ActionCalculator.Models.Actions
{
	public class Injury : Action
	{
		public Injury(int roll) : base(ActionType.Injury, 0, 0, roll, false)
		{
		}
	}
}