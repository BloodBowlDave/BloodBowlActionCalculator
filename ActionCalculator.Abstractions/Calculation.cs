using System.Text;

namespace ActionCalculator.Abstractions
{
    public class Calculation
    {
        public Calculation(PlayerAction[] playerActions)
        {
            PlayerActions = playerActions;
        }

        public PlayerAction[] PlayerActions { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var previousPlayerId = Guid.Empty;
            var firstAction = true;

            foreach (var playerAction in PlayerActions)
            {
                var player = playerAction.Player;
                var action = playerAction.Action;
                var currentPlayerId = player.Id;
                var requiresNonCriticalFailure = action.RequiresNonCriticalFailure;
                var requiresDauntlessFailure = action.RequiresDauntlessFailure;
                var branchTerminatesCalculation = BranchTerminatesCalculation(playerAction);
                var isStartOfAlternateBranch = IsStartOfAlternateBranch(playerAction);

                if (previousPlayerId != currentPlayerId)
                {
                    firstAction = true;

                    var previousPlayerHasSubsequentAction = PreviousPlayerHasSubsequentAction(previousPlayerId, playerAction);
                    var currentPlayerHasPreviousAction = CurrentPlayerHasPreviousAction(currentPlayerId, playerAction);

                    if (requiresNonCriticalFailure)
                    {
                        if (!previousPlayerHasSubsequentAction)
                        {
                            sb.Append(";");
                        }

                        sb.Append(branchTerminatesCalculation ? '[' : '{');
                    }
                    else if (previousPlayerId != Guid.Empty && !currentPlayerHasPreviousAction)
                    {
                        sb.Append(";");
                    }

                    if (isStartOfAlternateBranch)
                    {
                        sb.Append("(");
                    }
                    
                    if (!currentPlayerHasPreviousAction)
                    {
                        sb.Append(player);

                        if (player.HasSkills() || requiresNonCriticalFailure && previousPlayerHasSubsequentAction)
                        {
                            sb.Append(':');
                        }
                    }
                }
                else if (requiresNonCriticalFailure && !requiresDauntlessFailure)
                {
                    sb.Append(branchTerminatesCalculation ? '[' : '{');
                }
                
                if (firstAction)
                {
                    firstAction = false;
                }
                else
                {
                    if (requiresDauntlessFailure)
                    {
                        sb.Append('|');
                    }
                    else if (isStartOfAlternateBranch)
                    {
                        sb.Append(playerAction.BranchId == 1 ? "(" : ")(");
                    }
                    else
                    {
                        if (!CharacterIsABracket(sb.ToString().Last()))
                        {
                            sb.Append(',');
                        }
                    }
                }

                sb.Append(action);

                var isEndOfAlternateBranches = IsEndOfAlternateBranches(playerAction);

                if (action.EndOfBranch)
                {
                    var depthDifference = playerAction.Depth - DepthOfNextAction(playerAction);

                    for (var i = 0; i < depthDifference; i++)
                    {
                        sb.Append(action.TerminatesCalculation ? ']' : '}');
                    }
                }
                else if (isEndOfAlternateBranches)
                {
                    sb.Append(')');
                }

                previousPlayerId = currentPlayerId;
            }

            return sb.ToString();
        }

        private int DepthOfNextAction(PlayerAction playerAction) => 
            PlayerActions.FirstOrDefault(x => x.Index > playerAction.Index)?.Depth ?? 0;

        private static bool CharacterIsABracket(char character) =>
            new List<char> {'}', ']', '{', '[', '(', ')'}.Contains(character);

        private bool CurrentPlayerHasPreviousAction(Guid currentPlayerId, PlayerAction playerAction) => 
            PlayerActions.FirstOrDefault(x => x.Player.Id == currentPlayerId && x.Index < playerAction.Index) != null;

        private bool PreviousPlayerHasSubsequentAction(Guid previousPlayerId, PlayerAction playerAction) => 
            PlayerActions.FirstOrDefault(x => x.Player.Id == previousPlayerId && x.Index > playerAction.Index) != null;

        private bool IsEndOfAlternateBranches(PlayerAction playerAction) => 
            playerAction.BranchId > 0 && (PlayerActions.FirstOrDefault(x => x.Index > playerAction.Index)?.BranchId ?? 0) == 0;

        private bool IsStartOfAlternateBranch(PlayerAction playerAction) => 
            playerAction.BranchId > 0 && !PlayerActions.Any(x => x.BranchId == playerAction.BranchId && x.Index < playerAction.Index);

        private bool BranchTerminatesCalculation(PlayerAction playerAction) => 
            PlayerActions.LastOrDefault(x => x.Index > playerAction.Index && x.Depth >= playerAction.Depth)?.Action.TerminatesCalculation ?? false;
    }
}