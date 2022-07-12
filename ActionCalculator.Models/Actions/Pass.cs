namespace ActionCalculator.Models.Actions
{
	public class Pass : Action
	{
		public Pass(int roll, int modifier, bool usePro, bool rerollInaccuratePass) : base(ActionType.Pass, roll, usePro)
		{
			RerollInaccuratePass = rerollInaccuratePass;
			Modifier = modifier;
		}
    
		public bool RerollInaccuratePass { get; }
        public int Modifier { get; }
		
		public override string ToString() => $"{(char) ActionType}{Numerator}{GetModifier()}{(!RerollInaccuratePass ? "'" : "")}";

		public override string GetDescription() => base.GetDescription() + $" {GetModifier()}";

		private string GetModifier() => Modifier > 0 ? "+" + Modifier + " Modifier" : Modifier < 0 ? Modifier + " Modifier" : "";
	}
}