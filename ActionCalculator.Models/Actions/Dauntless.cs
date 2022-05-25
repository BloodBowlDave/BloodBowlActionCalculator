namespace ActionCalculator.Models.Actions;

public class Dauntless : Action
{
    public Dauntless(decimal success, decimal failure, bool rerollFailure, int roll, bool usePro) 
        : base(ActionType.Dauntless, success, failure, roll, usePro)
    {
        RerollFailure = rerollFailure;
    }
    
    public bool RerollFailure { get; }

    public override string ToString() => $"{(char) ActionType}{Roll}{(!RerollFailure ? "'" : "")}{(UsePro ? "*" : "")}";
}