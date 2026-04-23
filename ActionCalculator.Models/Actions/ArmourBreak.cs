namespace ActionCalculator.Models.Actions
{
	public class ArmourBreak(int roll) : Action(ActionType.ArmourBreak, roll, false)
	{
        public override string ToString() => $"{(char) ActionType}{Roll}";

		public override bool IsRerollable() => false;
	}
}