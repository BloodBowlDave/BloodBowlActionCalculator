namespace ActionCalculator.Models.Actions
{
	public class Landing : Action
	{
		public Landing(int roll, bool usePro) : base(ActionType.Landing, roll, usePro)
		{
		}
	}
}