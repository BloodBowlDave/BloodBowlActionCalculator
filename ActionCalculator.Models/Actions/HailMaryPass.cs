namespace ActionCalculator.Models.Actions
{
	public class HailMaryPass : Action
	{
		public HailMaryPass(decimal success, decimal failure, int roll, bool usePro) 
			: base(ActionType.HailMaryPass, success, failure, roll, usePro)
		{
		}
	}
}