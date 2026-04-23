namespace ActionCalculator.Models.Actions
{
	public class Foul(int roll) : Action(ActionType.Foul, roll, false)
	{
        public override bool IsRerollable() => false;
	}
}