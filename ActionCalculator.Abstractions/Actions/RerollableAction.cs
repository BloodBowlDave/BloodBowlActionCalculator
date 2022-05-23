namespace ActionCalculator.Abstractions.Actions;

public class RerollableAction : Action
{
    public RerollableAction(decimal success, decimal failure, bool usePro, int roll) 
        : base(ActionType.Rerollable, success, failure, roll, usePro)
    {
    }
    
    public override string ToString() => Roll + (UsePro ? "*" : "");
}