namespace ActionCalculator.Models.Actions
{
	public class ThrowTeammate(int roll, int modifier, bool usePro, bool rerollInaccurateThrow) : Action(ActionType.ThrowTeammate, roll, usePro)
	{
        public bool RerollInaccurateThrow { get; set; } = rerollInaccurateThrow;
        public int Modifier { get; } = modifier;

        public override string ToString() => $"{(char)ActionType}{Roll}{GetModifier()}{(!RerollInaccurateThrow ? "'" : "")}";

        public override string GetDescription() => base.GetDescription() + (Modifier != 0 ? $" {GetModifier() + " Modifier"}" : "");

        private string GetModifier() => Modifier > 0 ? "+" + Modifier : Modifier < 0 ? Modifier.ToString() : "";
	}
}