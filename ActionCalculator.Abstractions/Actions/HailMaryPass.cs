namespace ActionCalculator.Abstractions.Actions;

public class HailMaryPass : Action
{
    protected HailMaryPass(decimal success, decimal failure, int roll, bool usePro) 
        : base(ActionType.HailMaryPass, success, failure, roll, usePro)
    {
    }
}