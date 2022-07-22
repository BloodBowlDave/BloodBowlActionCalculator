using System.Text;

namespace ActionCalculator.Models
{
    public class PlayerActions : List<PlayerAction>
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            var previousPlayerId = Guid.Empty;
            var firstAction = true;

            for (var i = 0; i < Count; i++)
            {
                var playerAction = this[i];
                var player = playerAction.Player;
                var action = playerAction.Action;
                var currentPlayerId = player.Id;
                var depth = playerAction.Depth;
                var branchId = playerAction.BranchId;

                var requiresNonCriticalFailure = playerAction.RequiresNonCriticalFailure;
                var requiresDauntlessFailure = RequiresDauntlessFailure(i);
                var branchTerminatesCalculation = BranchTerminatesCalculation(i, depth);
                var isStartOfAlternateBranch = branchId > 0 && IsStartOfAlternateBranch(i, branchId);

                if (previousPlayerId != currentPlayerId)
                {
                    firstAction = true;

                    var previousPlayerHasSubsequentAction =
                        PreviousPlayerHasSubsequentAction(previousPlayerId, i);
                    var currentPlayerHasPreviousAction = CurrentPlayerHasPreviousAction(currentPlayerId, i);
                    var endPlayer = !previousPlayerHasSubsequentAction && !branchTerminatesCalculation;

                    if (requiresNonCriticalFailure)
                    {
                        if (endPlayer)
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

                        if (player.HasAnySkills() || requiresNonCriticalFailure && !endPlayer)
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
                        sb.Append(branchId == 1 ? "(" : ")(");
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

                var isEndOfAlternateBranches = branchId > 0 && IsEndOfAlternateBranches(i);

                if (playerAction.EndOfBranch)
                {
                    var depthDifference = depth - DepthOfNextAction(i);

                    for (var j = 0; j < depthDifference; j++)
                    {
                        sb.Append(playerAction.TerminatesCalculation ? ']' : '}');
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
        
        private bool RequiresDauntlessFailure(int i) => i > 1 && this[i - 2].Action.ActionType == ActionType.Dauntless;

        private int DepthOfNextAction(int i) => i + 1 < Count ? this[i + 1].Depth : 0;

        private static bool CharacterIsABracket(char character) =>
            new List<char> { '}', ']', '{', '[', '(', ')' }.Contains(character);

        private bool CurrentPlayerHasPreviousAction(Guid currentPlayerId, int i)
        {
            for (var j = i - 1; j >= 0; j--)
            {
                if (this[j].Player.Id == currentPlayerId)
                {
                    return true;
                }
            }

            return false;
        }

        private bool PreviousPlayerHasSubsequentAction(Guid previousPlayerId, int i)
        {
            for (var j = i + 1; j < Count; j++)
            {
                if (this[j].Player.Id == previousPlayerId)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsEndOfAlternateBranches(int i) => i + 1 == Count || this[i + 1].BranchId == 0;

        private bool IsStartOfAlternateBranch(int i, int branchId)
        {
            for (var j = 0; j < i; j++)
            {
                if (this[j].BranchId == branchId)
                {
                    return false;
                }
            }

            return true;
        }

        private bool BranchTerminatesCalculation(int i, int depth)
        {
            for (var j = Count - 1; j >= i; j--)
            {
                var playerAction = this[j];
                if (playerAction.Depth >= depth)
                {
                    return playerAction.TerminatesCalculation;
                }
            }

            return false;
        }
    }
}