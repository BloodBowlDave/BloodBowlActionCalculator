namespace ActionCalculator.Abstractions.Actions;

public class Pass : Action
{
    public Pass(decimal success, decimal failure, decimal inaccuratePass, bool usePro, bool rerollInaccuratePass, int roll, int modifier) 
        : base(ActionType.Pass, success, failure, roll, usePro)
    {
        InaccuratePass = inaccuratePass;
        RerollInaccuratePass = rerollInaccuratePass;
        Modifier = modifier;
    }
    
    public decimal InaccuratePass { get; }
    public bool RerollInaccuratePass { get; }
    private int Modifier { get; }

    public override string ToString() => $"{(char) ActionType}{Roll}{GetModifier()}{(!RerollInaccuratePass ? "'" : "")}";

    private string GetModifier() => Modifier > 0 ? "+" + Modifier : Modifier.ToString();
}