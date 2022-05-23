namespace ActionCalculator.Abstractions.Actions;

public class Foul : Action
{
    public Foul(int roll) : base(ActionType.Foul, 0, 0, roll, false)
    {
    }
}