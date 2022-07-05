﻿namespace ActionCalculator.Models.Actions
{
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
		public int Modifier { get; }

		public override string ToString() => $"{(char) ActionType}{Roll}{GetModifier()}{(!RerollInaccuratePass ? "'" : "")}";

		public override string GetDescription() => base.GetDescription() + $" {GetModifier()} Modifier";

		private string GetModifier() => Modifier > 0 ? "+" + Modifier : Modifier < 0 ? Modifier.ToString() : "";
	}
}