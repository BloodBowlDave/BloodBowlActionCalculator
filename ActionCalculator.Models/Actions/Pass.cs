namespace ActionCalculator.Models.Actions
{
	public class Pass : Action
	{
		public Pass(bool usePro, bool rerollInaccuratePass, int roll, int modifier) : base(ActionType.Pass, roll, usePro)
		{
			RerollInaccuratePass = rerollInaccuratePass;
			Modifier = modifier;
		}
    
		public bool RerollInaccuratePass { get; }
        public int Modifier { get; }

		public override string ToString() => $"{(char) ActionType}{Roll}{GetModifier()}{(!RerollInaccuratePass ? "'" : "")}";

		public override string GetDescription() => base.GetDescription() + $" {GetModifier()} Modifier";

		private string GetModifier() => Modifier > 0 ? "+" + Modifier : Modifier < 0 ? Modifier.ToString() : "";
	}
}