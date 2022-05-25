namespace ActionCalculator.Models.Actions
{
    public class Catch : Action
    {
        public Catch(decimal success, decimal failure, int roll, bool usePro) 
            : base(ActionType.Catch, success, failure, roll, usePro)
        {
        }
    }
}
