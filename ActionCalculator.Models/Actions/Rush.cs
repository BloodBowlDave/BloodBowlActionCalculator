namespace ActionCalculator.Models.Actions
{
	public class Rush : Action
	{
		public Rush(decimal success, decimal failure, int roll, bool usePro) 
			: base(ActionType.Rush, success, failure, roll, usePro)
		{
		}
	}
}