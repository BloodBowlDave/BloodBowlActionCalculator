namespace ActionCalculator.Models.Actions
{
	public class Landing : Action
	{
		public Landing(decimal success, decimal failure, int roll, bool usePro) 
			: base(ActionType.Landing, success, failure, roll, usePro)
		{
		}
	}
}