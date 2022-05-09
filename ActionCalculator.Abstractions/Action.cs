namespace ActionCalculator.Abstractions
{
    public class Action : IAction
    {
        public Action(ActionType actionType, decimal success, decimal failure, decimal nonCriticalFailure, decimal successOnOneDie)
        {
            ActionType = actionType;
            Success = success;
            Failure = failure;
            NonCriticalFailure = nonCriticalFailure;
            SuccessOnOneDie = successOnOneDie;
        }

        public ActionType ActionType { get; }
        public decimal Success { get; }
        public decimal Failure { get; }
        public decimal NonCriticalFailure { get; }
        public decimal SuccessOnOneDie { get; }
        public bool UsePro { get; set; }
        public bool UseBrawler { get; set; }
        public bool UseBreakTackle { get; set; }
        public bool RerollNonCriticalFailure { get; set; }
        public bool UseDivingTackle { get; set; }
        public int OriginalRoll { get; init; }
        public int Modifier { get; init; }
        public int NumberOfDice { get; init; }
        public int NumberOfSuccessfulResults { get; init; }
        public decimal SuccessOnTwoDice { get; init; }
        public bool RequiresNonCriticalFailure { get; set; }
        public bool TerminatesCalculation { get; set; }
        public bool RequiresDauntlessFailure { get; set; }
        public bool EndOfBranch { get; set; }

        public override string ToString() =>
            ActionType switch
            {
                ActionType.Block => $"{NumberOfDice}D{NumberOfSuccessfulResults}{(UseBrawler ? "^" : "")}{(UsePro ? "*" : "")}",
                ActionType.Rerollable => OriginalRoll.ToString(),
                ActionType.Bribe => ((char)ActionType).ToString(),
                ActionType.Shadowing => $"{(char)ActionType}{GetModifier()}{(!RerollNonCriticalFailure ? "'" : "")}",
                ActionType.Tentacles => $"{(char)ActionType}{GetModifier()}{(!RerollNonCriticalFailure ? "'" : "")}",
                ActionType.Pass => $"{(char)ActionType}{OriginalRoll}{GetModifier()}{(!RerollNonCriticalFailure ? "'" : "")}",
                ActionType.ThrowTeamMate => $"{(char)ActionType}{OriginalRoll}{GetModifier()}{(!RerollNonCriticalFailure ? "'" : "")}",
                ActionType.NonRerollable => $"{(char)ActionType}{OriginalRoll}{(Modifier > 0 ? "/" + Modifier : "")}",
                ActionType.Dodge => $"{(char)ActionType}{OriginalRoll}{(!UseBreakTackle ? "¬" : "")}{(UseDivingTackle ? "\"" : "")}{(UsePro ? "*" : "")}",
                _ => $"{(char)ActionType}{OriginalRoll}{(!RerollNonCriticalFailure ? "'" : "")}{(UsePro ? "*" : "")}"
            };

        private string GetModifier() => Modifier > 0 ? '+' + Modifier.ToString() : Modifier < 0 ? Modifier.ToString() : "";

        public void Deconstruct(out decimal success, out decimal failure)
        {
            success = Success;
            failure = Failure;
        }
    }
}
