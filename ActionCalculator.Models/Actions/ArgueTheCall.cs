namespace ActionCalculator.Models.Actions
{
	public class ArgueTheCall(int roll) : Action(ActionType.ArgueTheCall, roll, false)
	{
        public override bool IsRerollable() => false;
	}
}