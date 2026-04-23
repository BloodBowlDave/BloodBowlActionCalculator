namespace ActionCalculator.Models.Actions
{
    public class Chainsaw(int roll) : Action(ActionType.Chainsaw, roll, false)
    {
        public override bool IsRerollable() => false;
    }
}
