namespace ActionCalculator.Models.Actions
{
	public class PickUp(int roll, bool usePro) : Action(ActionType.PickUp, roll, usePro)
	{
    }
}