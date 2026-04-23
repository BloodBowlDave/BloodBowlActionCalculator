namespace ActionCalculator.Models.Actions
{
    public class Stab(int roll) : Action(ActionType.Stab, roll, false)
    {
        public override bool IsRerollable() => false;
    }
}
