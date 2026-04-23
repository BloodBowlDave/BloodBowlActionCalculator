namespace ActionCalculator.Models.Actions
{
	public class Injury(int roll) : Action(ActionType.Injury, roll, false)
	{
        public override bool IsRerollable() => false;
	}
}