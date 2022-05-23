namespace ActionCalculator.Abstractions.Actions;

public class Rush : Action
{
    protected Rush(decimal success, decimal failure, int roll, bool usePro) 
        : base(ActionType.Rush, success, failure, roll, usePro)
    {
    }
}