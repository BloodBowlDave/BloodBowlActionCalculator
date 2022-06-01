using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class ActionMediator : IActionMediator
    {
        private readonly IActionStrategyFactory _actionStrategyFactory;
        private CalculationContext _context;

        public ActionMediator(IActionStrategyFactory actionStrategyFactory)
        {
            _actionStrategyFactory = actionStrategyFactory;
            _context = null!;
        }

        public void Initialise(CalculationContext context)
        {
            _context = context;
        }

        public void Resolve(decimal p, int r, int i, Skills usedSkills, bool nonCriticalFailure = false)
        {
            if (p == 0 || r < 0)
            {
                return;
            }

            var previousPlayerAction = _context.Calculation.PlayerActions.SingleOrDefault(x => x.Index == i);
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

            if (nonCriticalFailure)
            {
                if (PlayerSentOff(previousActionType, action.ActionType) || !NonCriticalFailureSupported(previousActionType)
                    && !playerAction.RequiresNonCriticalFailure)
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
                Execute(playerAction, p, r, usedSkills, nonCriticalFailure);
                return;
            }

            while (playerAction != null)
            {
                Execute(playerAction, p, r, GetUsedSkills(previousPlayerAction?.Player.Id, player.Id, usedSkills), nonCriticalFailure);
                playerAction = GetNextBranchStartPlayerAction(i, playerAction.BranchId);
            }
        }

        private bool SkipPlayerAction(bool nonCriticalFailure, PlayerAction? previousPlayerAction) =>
            previousPlayerAction != null && _context.Calculation.PlayerActions.SingleOrDefault(x 
                => x.Index == previousPlayerAction.Index - 1)?.Action.ActionType == ActionType.Dauntless
            || previousPlayerAction?.Action.ActionType == ActionType.Dauntless && nonCriticalFailure;

        private PlayerAction GetNextPlayerAction(int i, bool skipPlayerAction) => _context.Calculation.PlayerActions[i + (skipPlayerAction ? 2 : 1)];

        private static bool IsStartOfBranch(PlayerAction? previousPlayerAction, PlayerAction playerAction) =>
            playerAction.BranchId != 0 && playerAction.BranchId != previousPlayerAction?.BranchId;

        private bool IsEndOfCalculation(PlayerAction playerAction, bool skipPlayerAction) =>
            playerAction.Index + (skipPlayerAction ? 2 : 1) == _context.Calculation.PlayerActions.Length 
            || playerAction.TerminatesCalculation;

        private PlayerAction? GetNextBranchStartPlayerAction(int i, int branchId) =>
            _context.Calculation.PlayerActions.FirstOrDefault(x => x.Index > i && x.BranchId > branchId);
        
        private static bool IsEndOfBranch(int? previousBranchId, int branchId) => branchId > 0 && previousBranchId > 0 && previousBranchId != branchId;

        private PlayerAction? GetNextNonBranchPlayerAction(int i) =>
            _context.Calculation.PlayerActions.FirstOrDefault(x => x.Index > i && x.BranchId == 0);

        private static Skills GetUsedSkills(Guid? previousPlayerId, Guid playerId, Skills usedSkills) =>
            previousPlayerId != playerId ? usedSkills & Skills.DivingTackle & Skills.BlastIt & Skills.CloudBurster : usedSkills;

        private PlayerAction? GetNextValidPlayerAction(PlayerAction playerAction) =>
            _context.Calculation.PlayerActions.FirstOrDefault(x =>
                x.Depth < playerAction.Depth && x.Index > playerAction.Index);

        private static bool NonCriticalFailureSupported(ActionType? previousActionType) =>
            previousActionType is ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury or ActionType.Foul or ActionType.HailMaryPass
                or ActionType.Pass or ActionType.ThrowTeamMate or ActionType.Interception or ActionType.NonRerollable or ActionType.Dauntless;

        private static bool PlayerSentOff(ActionType? previousActionType, ActionType actionType) =>
            previousActionType switch
            {
                ActionType.Foul => actionType is not (ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury),
                ActionType.ArgueTheCall => actionType is not ActionType.Bribe,
                ActionType.Bribe => actionType is not ActionType.ArgueTheCall,
                ActionType.Injury => actionType is not (ActionType.Bribe or ActionType.ArgueTheCall),
                _ => false
            };

        private void Execute(PlayerAction playerAction, decimal p, int r, Skills usedSkills, bool nonCriticalFailure)
        {
            var actionStrategy = _actionStrategyFactory.GetActionStrategy(playerAction.Action, this, nonCriticalFailure);
            actionStrategy.Execute(p, r, playerAction, usedSkills, nonCriticalFailure);
        }

        private void WriteResult(decimal p, int r, Skills usedSkills, ActionType? previousActionType)
        {
            Console.WriteLine($"MaxRerolls:{_context.MaxRerolls} P:{p:0.00000} R:{r} Action:{previousActionType} UsedSkills:{usedSkills}");

            _context.Results[_context.MaxRerolls - r] += p;
        }
    }
}
