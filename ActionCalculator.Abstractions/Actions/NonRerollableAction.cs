namespace ActionCalculator.Abstractions.Actions;

public class NonRerollableAction : Action
{
    public NonRerollableAction(decimal success, decimal failure, int roll, int denominator) 
        : base(ActionType.NonRerollable, success, failure, roll, false)
    {
        Denominator = denominator;
    }
    
    public int Denominator { get; }

    public override string ToString() => $"{(char) ActionType}{Roll}{(Denominator > 0 ? "/" + Denominator : "")}";
}