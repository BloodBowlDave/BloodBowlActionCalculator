﻿namespace ActionCalculator.Models.Actions
{
	public class ThrowTeammate : Action
	{
		public ThrowTeammate(int roll, int modifier, bool usePro, bool rerollInaccurateThrow) 
			: base(ActionType.ThrowTeammate, roll, usePro)
		{
			RerollInaccurateThrow = rerollInaccurateThrow;
			Modifier = modifier;
		}
    
		public bool RerollInaccurateThrow { get; }
        public int Modifier { get; }

		public override string ToString() => $"{(char)ActionType}{Roll}{GetModifier()}{(!RerollInaccurateThrow ? "'" : "")}";
		
		public override string GetDescription() => base.GetDescription() + $" {GetModifier()} Modifier";

		private string GetModifier() => Modifier > 0 ? "+" + Modifier : Modifier < 0 ? Modifier.ToString() : "";
	}
}