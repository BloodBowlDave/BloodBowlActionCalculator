namespace ActionCalculator.Models.Actions
{
	public class Interference(int roll) : Action(ActionType.Interference, roll, false)
	{
        public override bool IsRerollable() => false;
	}
}