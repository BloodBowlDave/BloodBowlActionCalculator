namespace ActionCalculator.Models.Actions
{
    public class Catch : Action
    {
        public Catch(int roll, bool usePro) : base(ActionType.Catch, roll, usePro)
        {
        }
    }
}
