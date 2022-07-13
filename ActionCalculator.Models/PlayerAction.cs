using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Models
{
    public class PlayerAction
    {
        public PlayerAction(Player player, Action action, int depth, int index)
        {
            Player = player;
            Action = action;
            Depth = depth;
            Index = index;
        }

        public Player Player { get; set; }
        public Action Action { get; }
        public int Depth { get; }
        public int Index { get; }
        public int BranchId { get; set; }
        public bool RequiresNonCriticalFailure { get; set; }
        public bool EndOfBranch { get; set; }
        public bool TerminatesCalculation { get; set; }
    }
}
