namespace ActionCalculator.Models.Actions
{
	public class ThrowTeammate : Action
	{
		public ThrowTeammate(decimal success, decimal failure, decimal inaccurateThrow, bool usePro, 
			bool rerollInaccurateThrow, int roll, int modifier) 
			: base(ActionType.ThrowTeammate, success, failure, roll, usePro)
		{
			InaccurateThrow = inaccurateThrow;
			RerollInaccurateThrow = rerollInaccurateThrow;
			Modifier = modifier;
		}
    
		public decimal InaccurateThrow { get; }
		public bool RerollInaccurateThrow { get; }
		public int Modifier { get; }

		public override string ToString() => $"{(char)ActionType}{Roll}{GetModifier()}{(!RerollInaccurateThrow ? "'" : "")}";

		private string GetModifier() => Modifier > 0 ? "+" + Modifier : Modifier < 0 ? Modifier.ToString() : "";
	}
}