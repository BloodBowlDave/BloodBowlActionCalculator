using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class ActionMediator : IActionMediator
    {
        private readonly IStrategyFactory _strategyFactory;
        private Calculation _calculation;
        private decimal[] _results;

        public ActionMediator(IStrategyFactory strategyFactory)
        {
            _strategyFactory = strategyFactory;
            _calculation = null!;
            _results = null!;
        }

        public decimal[] Calculate(Calculation calculation)
        {
            if (!calculation.PlayerActions.Any())
            {
                return Array.Empty<decimal>();
            }

            _calculation = calculation;
            _results = new decimal[calculation.Rerolls * 2 + 1];

            Resolve(1m, calculation.Rerolls, -1, Skills.None);
            AggregateResults(_results);

            return _results.Where(x => x > 0).ToArray();
        }

        public void Resolve(decimal p, int r, int i, Skills usedSkills, bool nonCriticalFailure = false)
        {
            if (p == 0 || r < 0)
            {
                return;
            }

            var previousPlayerAction = i > -1 ? _calculation.PlayerActions[i] : null;
            var previousActionType = previousPlayerAction?.Action.ActionType;

            var skipPlayerAction = SkipPlayerAction(nonCriticalFailure, i);
            
            if (previousPlayerAction != null && IsEndOfCalculation(i, skipPlayerAction))
            {
                if (previousActionType is ActionType.Tentacles or ActionType.Block or ActionType.Stab or ActionType.Chainsaw && nonCriticalFailure)
                {
                    return;
                }

                WriteResult(p, r, usedSkills, previousActionType);
                return;
            }
            
            i = GetNextPlayerActionIndex(i, skipPlayerAction);

            var playerAction = _calculation.PlayerActions[i];
            var player = playerAction.Player;
            var action = playerAction.Action;

            if (previousActionType == ActionType.Foul && !nonCriticalFailure && action.ActionType is not ActionType.Injury)
            {
                Resolve(p / 6, r, i - 1, usedSkills, true);
                p *= 5m / 6;
            }

            if (nonCriticalFailure)
            {
                if (PlayerSentOff(previousActionType, action.ActionType))
                {
                    return;
                }

                if (!NonCriticalFailureSupported(previousActionType) && !playerAction.RequiresNonCriticalFailure)
                {
                    return;
                }
            }
            else if (playerAction.RequiresNonCriticalFailure)
            {
                i = GetNextPlayerActionIndexOnMainBranch(playerAction, i);

                if (i == -1)
                {
                    WriteResult(p, r, usedSkills, previousActionType);
                    return;
                }

                playerAction = _calculation.PlayerActions[i];
            }
            else if (IsEndOfBranch(previousPlayerAction?.BranchId, playerAction.BranchId))
            {
                i = GetNextNonBranchPlayerActionIndex(i);
                
                if (i == -1)
                {
                    WriteResult(p, r, usedSkills, previousActionType);
                    return;
                }

                playerAction = _calculation.PlayerActions[i];
            }

            if (!IsStartOfBranch(previousPlayerAction, playerAction))
            {
                usedSkills = GetUsedSkills(previousPlayerAction?.Player.Id, player.Id, usedSkills);
                Execute(playerAction, p, r, i, usedSkills, previousActionType, nonCriticalFailure);
                return;
            }

            while (true)
            {
                Execute(playerAction, p, r, i, GetUsedSkills(previousPlayerAction?.Player.Id, player.Id, usedSkills), previousActionType, nonCriticalFailure);

                i = GetNextBranchStartPlayerActionIndex(i, playerAction.BranchId);

                if (i == -1)
                {
                    break;
                }

                playerAction = _calculation.PlayerActions[i];
            }
        }
        
        private bool SkipPlayerAction(bool nonCriticalFailure, int i) =>
            i > -1 && (nonCriticalFailure &&
                       _calculation.PlayerActions[i].Action.ActionType == ActionType.Dauntless ||
                       i > 0 && _calculation.PlayerActions[i - 1].Action.ActionType == ActionType.Dauntless);

        private static int GetNextPlayerActionIndex(int i, bool skipPlayerAction) => i + (skipPlayerAction ? 2 : 1);

        private static bool IsStartOfBranch(PlayerAction? previousPlayerAction, PlayerAction playerAction) =>
            playerAction.BranchId != 0 && playerAction.BranchId != previousPlayerAction?.BranchId;

        private bool IsEndOfCalculation(int i, bool skipPlayerAction) =>
            i + (skipPlayerAction ? 2 : 1) == _calculation.PlayerActions.Count || _calculation.PlayerActions[i].TerminatesCalculation;

        private int GetNextBranchStartPlayerActionIndex(int i, int branchId)
        {
            for (var j = i + 1; j < _calculation.PlayerActions.Count; j++)
            {
                if (_calculation.PlayerActions[j].BranchId > branchId)
                {
                    return j;
                }
            }

            return -1;
        }

        private static bool IsEndOfBranch(int? previousBranchId, int branchId) => branchId > 0 && previousBranchId > 0 && previousBranchId != branchId;

        private int GetNextNonBranchPlayerActionIndex(int i)
        {
            for (var j = i + 1; j < _calculation.PlayerActions.Count; j++)
            {
                if (_calculation.PlayerActions[j].BranchId == 0)
                {
                    return j;
                }
            }

            return -1;
        }

        private static Skills GetUsedSkills(Guid? previousPlayerId, Guid playerId, Skills usedSkills) =>
            previousPlayerId != playerId ? usedSkills & (Skills.DivingTackle | Skills.BlastIt | Skills.CloudBurster) : usedSkills;

        private int GetNextPlayerActionIndexOnMainBranch(PlayerAction? previousPlayerAction, int i)
        {
            for (var j = i + 1; j < _calculation.PlayerActions.Count; j++)
            {
                if (previousPlayerAction == null || _calculation.PlayerActions[j].Depth < previousPlayerAction.Depth)
                {
                    return j;
                }
            }

            return -1;
        }

        private static bool NonCriticalFailureSupported(ActionType? previousActionType) =>
            previousActionType is ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury or ActionType.Foul or ActionType.HailMaryPass
                or ActionType.Pass or ActionType.ThrowTeammate or ActionType.Interference or ActionType.NonRerollable or ActionType.Dauntless;

        private static bool PlayerSentOff(ActionType? previousActionType, ActionType? actionType) =>
            previousActionType switch
            {
                ActionType.Foul => actionType is not (ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury),
                ActionType.ArgueTheCall => actionType is not ActionType.Bribe,
                ActionType.Bribe => actionType is not ActionType.ArgueTheCall,
                ActionType.Injury => actionType is not (ActionType.Bribe or ActionType.ArgueTheCall),
                _ => false
            };

        private void Execute(PlayerAction playerAction, decimal p, int r, int i, Skills usedSkills, ActionType? previousActionType, bool nonCriticalFailure)
        {
            var actionStrategy = _strategyFactory.GetActionStrategy(playerAction.Action, this, previousActionType, nonCriticalFailure);
            actionStrategy.Execute(p, r, i, playerAction, usedSkills, nonCriticalFailure);
        }

        private void WriteResult(decimal p, int r, Skills usedSkills, ActionType? previousActionType)
        {
            Console.WriteLine($"Rerolls:{_calculation.Rerolls} P:{p:0.00000} R:{r} Action:{previousActionType} UsedSkills:{usedSkills}");

            _results[_calculation.Rerolls - r] += p;
        }

        private static void AggregateResults(IList<decimal> results)
        {
            for (var i = 1; i < results.Count; i++)
            {
                if (results[i] == 0)
                {
                    break;
                }

                results[i] += results[i - 1];
            }
        }
    }
}
