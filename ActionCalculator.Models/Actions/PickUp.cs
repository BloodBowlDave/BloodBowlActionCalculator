namespace ActionCalculator.Models.Actions
{
	public class PickUp : Action
	{
		public PickUp(bool usePro, int roll) : base(ActionType.PickUp, roll, usePro)
		{
		}
	}
}