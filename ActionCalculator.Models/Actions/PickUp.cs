namespace ActionCalculator.Models.Actions
{
	public class PickUp : Action
	{
		public PickUp(int roll, bool usePro) : base(ActionType.PickUp, roll, usePro)
		{
		}
	}
}