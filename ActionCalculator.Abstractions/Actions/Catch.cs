namespace ActionCalculator.Abstractions.Actions
{
    public abstract class Catch : Action
    {
        protected Catch(decimal success, decimal failure, int roll, bool usePro) 
            : base(ActionType.Catch, success, failure, roll, usePro)
        {
        }
    }
}
