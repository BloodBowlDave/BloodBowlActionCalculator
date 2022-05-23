namespace ActionCalculator.Abstractions.Actions;

public class Hypnogaze : Action
{
    protected Hypnogaze(decimal success, decimal failure, int roll, bool usePro, bool rerollFailure) 
        : base(ActionType.Hypnogaze, success, failure, roll, usePro)
    {
        RerollFailure = rerollFailure;
    }
    
    public bool RerollFailure { get; }
}