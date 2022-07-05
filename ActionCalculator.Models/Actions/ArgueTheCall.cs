namespace ActionCalculator.Models.Actions
{
	public class ArgueTheCall : Action
	{
		public ArgueTheCall(decimal success, decimal failure, decimal criticalFailure, int roll) 
			: base(ActionType.ArgueTheCall, success, failure, roll, false)
		{
			CriticalFailure = criticalFailure;
		}

		private decimal CriticalFailure { get; }

		public override bool IsRerollable() => false;
	}
}