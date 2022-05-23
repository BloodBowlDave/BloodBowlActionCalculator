namespace ActionCalculator.Abstractions.Actions;

public class Interception : Action
{
    protected Interception(decimal success, decimal failure, int roll, bool usePro) 
        : base(ActionType.Interception, success, failure, roll, usePro)
    {
    }
}