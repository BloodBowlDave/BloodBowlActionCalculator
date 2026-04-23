using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Models
{
    public class PlayerAction(Player player, Action action, int depth)
    {
        public Player Player { get; set; } = player;
        public Action Action { get; } = action;
        public int Depth { get; } = depth;
        public int BranchId { get; set; }
        public bool RequiresNonCriticalFailure { get; set; }
        public bool EndOfBranch { get; set; }
        public bool TerminatesCalculation { get; set; }
    }
}
