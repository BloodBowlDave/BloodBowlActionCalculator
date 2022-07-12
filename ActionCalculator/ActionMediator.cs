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

            var previousPlayerAction = _calculation.PlayerActions.SingleOrDefault(x => x.Index == i);
            var previousActionType = previousPlayerAction?.Action.ActionType;

            var skipPlayerAction = SkipPlayerAction(nonCriticalFailure, previousPlayerAction);
            
            if (previousPlayerAction != null && IsEndOfCalculation(previousPlayerAction, skipPlayerAction))
            {
                if (previousActionType is ActionType.Tentacles or ActionType.Block or ActionType.Stab or ActionType.Chainsaw && nonCriticalFailure)
                {
                    return;
                }

                WriteResult(p, r, usedSkills, previousActionType);
                return;
            }
            
            var playerAction = GetNextPlayerAction(i, skipPlayerAction);
            var player = playerAction.Player;
            var action = playerAction.Action;

            if (previousActionType == ActionType.Foul && !nonCriticalFailure && action.ActionType is not ActionType.Injury)
            {
                Resolve(p / 6, r, i, usedSkills, true);
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
                playerAction = GetNextValidPlayerAction(playerAction);

                if (playerAction == null)
                {
                    WriteResult(p, r, usedSkills, previousActionType);
                    return;
                }
            }
            else if (IsEndOfBranch(previousPlayerAction?.BranchId, playerAction.BranchId))
            {
                playerAction = GetNextNonBranchPlayerAction(i);

                if (playerAction == null)
                {
                    WriteResult(p, r, usedSkills, previousActionType);
                    return;
                }
            }

            if (!IsStartOfBranch(previousPlayerAction, playerAction))
            {
                usedSkills = GetUsedSkills(previousPlayerAction?.Player.Id, player.Id, usedSkills);
                Execute(playerAction, p, r, usedSkills, previousActionType, nonCriticalFailure);
                return;
            }

            while (playerAction != null)
            {
                Execute(playerAction, p, r, GetUsedSkills(previousPlayerAction?.Player.Id, player.Id, usedSkills), previousActionType, nonCriticalFailure);
                playerAction = GetNextBranchStartPlayerAction(i, playerAction.BranchId);
            }
        }
        
        private bool SkipPlayerAction(bool nonCriticalFailure, PlayerAction? previousPlayerAction) =>
            previousPlayerAction != null && _calculation.PlayerActions.SingleOrDefault(x 
                => x.Index == previousPlayerAction.Index - 1)?.Action.ActionType == ActionType.Dauntless
            || previousPlayerAction?.Action.ActionType == ActionType.Dauntless && nonCriticalFailure;

        private PlayerAction GetNextPlayerAction(int i, bool skipPlayerAction) => _calculation.PlayerActions[i + (skipPlayerAction ? 2 : 1)];

        private static bool IsStartOfBranch(PlayerAction? previousPlayerAction, PlayerAction playerAction) =>
            playerAction.BranchId != 0 && playerAction.BranchId != previousPlayerAction?.BranchId;

        private bool IsEndOfCalculation(PlayerAction playerAction, bool skipPlayerAction) =>
            playerAction.Index + (skipPlayerAction ? 2 : 1) == _calculation.PlayerActions.Count || playerAction.TerminatesCalculation;

        private PlayerAction? GetNextBranchStartPlayerAction(int i, int branchId) =>
            _calculation.PlayerActions.FirstOrDefault(x => x.Index > i && x.BranchId > branchId);
        
        private static bool IsEndOfBranch(int? previousBranchId, int branchId) => branchId > 0 && previousBranchId > 0 && previousBranchId != branchId;

        private PlayerAction? GetNextNonBranchPlayerAction(int i) =>
            _calculation.PlayerActions.FirstOrDefault(x => x.Index > i && x.BranchId == 0);

        private static Skills GetUsedSkills(Guid? previousPlayerId, Guid playerId, Skills usedSkills) =>
            previousPlayerId != playerId ? usedSkills & (Skills.DivingTackle | Skills.BlastIt | Skills.CloudBurster) : usedSkills;

        private PlayerAction? GetNextValidPlayerAction(PlayerAction playerAction) =>
            _calculation.PlayerActions.FirstOrDefault(x =>
                x.Depth < playerAction.Depth && x.Index > playerAction.Index);

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

        private void Execute(PlayerAction playerAction, decimal p, int r, Skills usedSkills, ActionType? previousActionType, bool nonCriticalFailure)
        {
            var actionStrategy = _strategyFactory.GetActionStrategy(playerAction.Action, this, previousActionType, nonCriticalFailure);
            actionStrategy.Execute(p, r, playerAction, usedSkills, nonCriticalFailure);
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
