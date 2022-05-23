namespace ActionCalculator.Abstractions.Actions;

public class ThrowTeamMate : Action
{
    public ThrowTeamMate(decimal success, decimal failure, decimal inaccurateThrow, bool usePro, 
        bool rerollInaccurateThrow, int roll, int modifier) 
        : base(ActionType.ThrowTeamMate, success, failure, roll, usePro)
    {
        InaccurateThrow = inaccurateThrow;
        RerollInaccurateThrow = rerollInaccurateThrow;
        Modifier = modifier;
    }
    
    public decimal InaccurateThrow { get; }
    public bool RerollInaccurateThrow { get; }
    private int Modifier { get; }

    public override string ToString() => $"{(char)ActionType}{Roll}{GetModifier()}{(!RerollInaccurateThrow ? "'" : "")}";

    private string GetModifier() => Modifier > 0 ? "+" + Modifier : Modifier.ToString();
}