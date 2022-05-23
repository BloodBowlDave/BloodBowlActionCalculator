namespace ActionCalculator.Abstractions
{
    public class PlayerAction
    {
        public PlayerAction(Player player, Action action, int depth)
        {
            Player = player;
            Action = action;
            Depth = depth;
        }

        public Player Player { get; }
        public Action Action { get; }
        public int Index { get; set; }
        public int Depth { get; }
        public int BranchId { get; set; }
        public bool RequiresNonCriticalFailure { get; set; }
        public bool EndOfBranch { get; set; }
        public bool TerminatesCalculation { get; set; }
    }
}
