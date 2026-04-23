namespace ActionCalculator.Models.Actions
{
	public class Pass(int roll, int modifier, bool usePro, bool rerollInaccuratePass) : Action(ActionType.Pass, roll, usePro)
	{
        public bool RerollInaccuratePass { get; set; } = rerollInaccuratePass;
        public int Modifier { get; } = modifier;

        public override string ToString() => $"{(char) ActionType}{Roll}{GetModifier()}{(!RerollInaccuratePass ? "'" : "")}";

		public override string GetDescription() => base.GetDescription() + (Modifier != 0 ? $" {GetModifier() + " Modifier"}" : "");

		private string GetModifier() => Modifier > 0 ? "+" + Modifier : Modifier < 0 ? Modifier.ToString() : "";
	}
}