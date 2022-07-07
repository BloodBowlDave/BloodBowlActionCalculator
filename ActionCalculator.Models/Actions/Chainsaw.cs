namespace ActionCalculator.Models.Actions
{
    public class Chainsaw : Action
    {
        public Chainsaw(int roll) : base(ActionType.Chainsaw, roll, false)
        {
        }

        public override bool IsRerollable() => false;
    }
}
