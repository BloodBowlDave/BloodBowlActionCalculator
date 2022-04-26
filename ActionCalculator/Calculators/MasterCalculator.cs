using System;
using System.Linq;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class MasterCalculator : IMasterCalculator
    {
        private readonly ICalculatorFactory _calculatorFactory;
        private CalculationContext _context;

        public MasterCalculator(ICalculatorFactory calculatorFactory)
        {
            _calculatorFactory = calculatorFactory;
        }

        public void Initialise(CalculationContext context)
        {
	        _context = context;
        }

        public void Calculate(decimal p, int r, PlayerAction previousPlayerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            if (p == 0)
            {
                return;
            }

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
                Calculate(playerAction, p, r, usedSkills, nonCriticalFailure);
                return;
            }

            while (playerAction != null)
            {
                Calculate(playerAction, p, r, GetUsedSkills(previousPlayerAction?.Player.Id, playerAction.Player.Id, usedSkills), nonCriticalFailure);
                playerAction = GetNextBranchStartPlayerAction(playerAction);
            }
        }

        private static bool IsStartOfBranch(PlayerAction previousPlayerAction, PlayerAction playerAction) => 
            playerAction.BranchId != 0 && playerAction.BranchId != previousPlayerAction?.BranchId;

        private bool IsEndOfCalculation(PlayerAction playerAction) =>
            playerAction.Index + 1 == _context.Calculation.PlayerActions.Length || playerAction.Action.TerminatesCalculation;

        private PlayerAction GetNextBranchStartPlayerAction(PlayerAction playerAction) =>
            _context.Calculation.PlayerActions.FirstOrDefault(x =>
                x.Index > playerAction.Index && x.BranchId > playerAction.BranchId);

        private PlayerAction GetNextPlayerAction(int? previousPlayerActionIndex, bool nonCriticalFailure, ActionType? previousActionType) => 
            _context.Calculation.PlayerActions[previousPlayerActionIndex + (nonCriticalFailure && previousActionType == ActionType.Dauntless ? 2 : 1) ?? 0];

        private static bool IsEndOfBranch(PlayerAction previousPlayerAction, PlayerAction playerAction) => 
            playerAction.BranchId > 0 && previousPlayerAction?.BranchId > 0 && previousPlayerAction.BranchId != playerAction.BranchId;

        private PlayerAction GetNextNonBranchPlayerAction(PlayerAction playerAction) => 
            _context.Calculation.PlayerActions.FirstOrDefault(x => x.Index > playerAction.Index && x.BranchId == 0);

        private static Skills GetUsedSkills(Guid? previousPlayerId, Guid playerId, Skills usedSkills) =>
            previousPlayerId != playerId ? usedSkills & Skills.DivingTackle : usedSkills;
        
        private PlayerAction GetNextValidPlayerAction(PlayerAction playerAction) => 
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

        private void Calculate(PlayerAction playerAction, decimal p, int r, Skills usedSkills, bool nonCriticalFailure)
        {
            var calculator = _calculatorFactory.CreateProbabilityCalculator(playerAction.Action, this, nonCriticalFailure);
            calculator.Calculate(p, r, playerAction, usedSkills, nonCriticalFailure);
        }

        private void WriteResult(decimal p, int r, Skills usedSkills, ActionType? previousActionType)
        {
            Console.WriteLine($"MaxRerolls:{_context.MaxRerolls} P:{p:0.00000} R:{r} Action:{previousActionType} UsedSkills:{usedSkills}");

            _context.Results[_context.MaxRerolls - r] += p;
        }
    }
}
