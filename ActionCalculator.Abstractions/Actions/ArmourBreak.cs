namespace ActionCalculator.Abstractions.Actions;

public class ArmourBreak : Action
{
    public ArmourBreak(int roll) : base(ActionType.ArmourBreak, 0m, 0m, roll, false)
    {
    }

    public override string ToString() => $"{(char) ActionType}{Roll}";
}