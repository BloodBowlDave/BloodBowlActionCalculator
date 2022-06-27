namespace ActionCalculator.Models.Actions
{
	public class Interception : Action
	{
		public Interception(decimal failure, int roll) 
			: base(ActionType.Interception, 0, failure, roll, false)
		{
		}
	}
}