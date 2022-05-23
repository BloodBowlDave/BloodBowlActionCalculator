namespace ActionCalculator.Abstractions.Actions;

public class Dauntless : Action
{
    protected Dauntless(decimal success, decimal failure, bool rerollFailure, int roll, bool usePro) 
        : base(ActionType.Dauntless, success, failure, roll, usePro)
    {
        RerollFailure = rerollFailure;
    }
    
    public bool RerollFailure { get; }
}