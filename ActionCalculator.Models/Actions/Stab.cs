namespace ActionCalculator.Models.Actions
{
    public class Stab : Action
    {
        public Stab(int roll) 
            : base(ActionType.Stab, 0, 0, roll, false)
        {
        }
    }
}
