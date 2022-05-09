using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class ActionMediator : IActionMediator
    {
        private readonly ICalculatorFactory _calculatorFactory;
        private CalculationContext _context;

        public ActionMediator(ICalculatorFactory calculatorFactory)
        {
            _calculatorFactory = calculatorFactory;
            _context = null!;
        }

        public void Initialise(CalculationContext context)
        {
            _context = context;
        }

        public void Resolve(decimal p, int r, int i, Skills usedSkills, bool nonCriticalFailure = false)
        {
            if (p == 0)
            {
                return;
            }

            var previousPlayerAction = _context.Calculation.PlayerActions.SingleOrDefault(x => x.Index == i);
            var previousActionType = previousPlayerAction?.Action.ActionType;

            if (previousPlayerAction != null && IsEndOfCalculation(previousPlayerAction))
            {
                if (previousActionType == ActionType.Tentacles && nonCriticalFailure)
                {
                    return;
                }

                WriteResult(p, r, usedSkills, previousActionType);
                return;
            }

            var playerAction = GetNextPlayerAction(previousPlayerAction?.Index, nonCriticalFailure, previousActionType);

            if (nonCriticalFailure)
            {
                if (PlayerSentOff(previousActionType, playerAction.Action.ActionType) || !NonCriticalFailureSupported(previousActionType)
                    && !playerAction.Action.RequiresNonCriticalFailure)
                {
                    return;
                }
            }
            else if (playerAction.Action.RequiresNonCriticalFailure)
            {
                playerAction = GetNextValidPlayerAction(playerAction);

                if (playerAction == null)
                {
                    WriteResult(p, r, usedSkills, previousActionType);
                    return;
                }
            }
            else if (IsEndOfBranch(previousPlayerAction, playerAction))
            {
                playerAction = GetNextNonBranchPlayerAction(playerAction);

                if (playerAction == null)
                {
                    WriteResult(p, r, usedSkills, previousActionType);
                    return;
                }
            }

            if (!IsStartOfBranch(previousPlayerAction, playerAction))
            {
                usedSkills = GetUsedSkills(previousPlayerAction?.Player.Id, playerAction.Player.Id, usedSkills);
                Execute(playerAction, p, r, usedSkills, nonCriticalFailure);
                return;
            }

            while (playerAction != null)
            {
                Execute(playerAction, p, r, GetUsedSkills(previousPlayerAction?.Player.Id, playerAction.Player.Id, usedSkills), nonCriticalFailure);
                playerAction = GetNextBranchStartPlayerAction(playerAction);
            }
        }

        private static bool IsStartOfBranch(PlayerAction? previousPlayerAction, PlayerAction playerAction) =>
            playerAction.BranchId != 0 && playerAction.BranchId != previousPlayerAction?.BranchId;

        private bool IsEndOfCalculation(PlayerAction playerAction) =>
            playerAction.Index + 1 == _context.Calculation.PlayerActions.Length || playerAction.Action.TerminatesCalculation;

        private PlayerAction? GetNextBranchStartPlayerAction(PlayerAction playerAction) =>
            _context.Calculation.PlayerActions.FirstOrDefault(x =>
                x.Index > playerAction.Index && x.BranchId > playerAction.BranchId);

        private PlayerAction GetNextPlayerAction(int? previousPlayerActionIndex, bool nonCriticalFailure, ActionType? previousActionType) =>
            _context.Calculation.PlayerActions[previousPlayerActionIndex + (nonCriticalFailure && previousActionType == ActionType.Dauntless ? 2 : 1) ?? 0];

        private static bool IsEndOfBranch(PlayerAction? previousPlayerAction, PlayerAction playerAction) =>
            playerAction.BranchId > 0 && previousPlayerAction?.BranchId > 0 && previousPlayerAction.BranchId != playerAction.BranchId;

        private PlayerAction? GetNextNonBranchPlayerAction(PlayerAction playerAction) =>
            _context.Calculation.PlayerActions.FirstOrDefault(x => x.Index > playerAction.Index && x.BranchId == 0);

        private static Skills GetUsedSkills(Guid? previousPlayerId, Guid playerId, Skills usedSkills) =>
            previousPlayerId != playerId ? usedSkills & Skills.DivingTackle & Skills.BlastIt & Skills.CloudBurster : usedSkills;

        private PlayerAction? GetNextValidPlayerAction(PlayerAction playerAction) =>
            _context.Calculation.PlayerActions.FirstOrDefault(x =>
                (x.Depth < playerAction.Depth || playerAction.Action.RequiresDauntlessFailure) && x.Index > playerAction.Index);

        private static bool NonCriticalFailureSupported(ActionType? previousActionType) =>
            previousActionType is ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury or ActionType.Foul or ActionType.HailMaryPass
                or ActionType.Pass or ActionType.ThrowTeamMate or ActionType.Interception or ActionType.NonRerollable;

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
            var actionStrategy = _calculatorFactory.GetActionStrategy(playerAction.Action, this, nonCriticalFailure);
            actionStrategy.Execute(p, r, playerAction, usedSkills, nonCriticalFailure);
        }

        private void WriteResult(decimal p, int r, Skills usedSkills, ActionType? previousActionType)
        {
            Console.WriteLine($"MaxRerolls:{_context.MaxRerolls} P:{p:0.00000} R:{r} Action:{previousActionType} UsedSkills:{usedSkills}");

            _context.Results[_context.MaxRerolls - r] += p;
        }
    }
}
