namespace ActionCalculator.Models.Actions
{
	public class PickUp : Action
	{
		public PickUp(decimal success, decimal failure, bool usePro, int roll) 
			: base(ActionType.PickUp, success, failure, roll, usePro)
		{
		}
	}
}