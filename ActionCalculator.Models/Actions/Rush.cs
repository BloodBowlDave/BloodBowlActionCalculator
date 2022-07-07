namespace ActionCalculator.Models.Actions
{
	public class Rush : Action
	{
		public Rush(int roll, bool usePro) 
			: base(ActionType.Rush, roll, usePro)
		{
		}
	}
}