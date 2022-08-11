namespace ActionCalculator.Models.Actions
{
	public class ArmourBreak : Action
	{
		public ArmourBreak(int roll) : base(ActionType.ArmourBreak, roll, false)
		{
		}

		public override string ToString() => $"{(char) ActionType}{Roll}";

		public override bool IsRerollable() => false;
	}
}