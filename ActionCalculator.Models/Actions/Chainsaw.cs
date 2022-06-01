namespace ActionCalculator.Models.Actions
{
    public class Chainsaw : Action
    {
        public Chainsaw(int roll) 
            : base(ActionType.Chainsaw, 0, 0, roll, false)
        {
        }
    }
}
