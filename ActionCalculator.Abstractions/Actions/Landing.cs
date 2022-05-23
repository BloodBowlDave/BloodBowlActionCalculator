namespace ActionCalculator.Abstractions.Actions;

public class Landing : Action
{
    protected Landing(decimal success, decimal failure, int roll, bool usePro) 
        : base(ActionType.Landing, success, failure, roll, usePro)
    {
    }
}