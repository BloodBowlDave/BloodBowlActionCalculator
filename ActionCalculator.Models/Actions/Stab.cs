namespace ActionCalculator.Models.Actions
{
    public class Stab : Action
    {
        public Stab(int roll) : base(ActionType.Stab, roll, false)
        {
        }

        public override bool IsRerollable() => false;
    }
}
