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

            var i = previousPlayerAction?.Index + (nonCriticalFailure && previousActionType == ActionType.Dauntless ? 2 : 1) ?? 0;
            var nextPlayerAction = _context.Calculation.PlayerActions[i];
            if (nonCriticalFailure)
            {
                if (PlayerSentOff(previousActionType, nextPlayerAction.Action.ActionType) 
                    || !NonCriticalFailureSupported(previousActionType) 
                    && !nextPlayerAction.Action.RequiresNonCriticalFailure)
                {
                    return;
                }
            }
            else if (nextPlayerAction.Action.RequiresDauntlessFailure)
            {
                nextPlayerAction = NextPlayerAction(previousActionType, i, nextPlayerAction);

                if (nextPlayerAction == null)
                {
                    WriteResult(p, r, usedSkills, previousActionType);
                    return;
                }
            }
            else if (nextPlayerAction.Action.RequiresNonCriticalFailure)
            {
                nextPlayerAction = GetNextPlayerAction(i, nextPlayerAction.Depth);

                if (nextPlayerAction == null)
                {
                    WriteResult(p, r, usedSkills, previousActionType);
                    return;
                }
            }

            if (previousPlayerAction?.Player.Id != nextPlayerAction.Player.Id)
            {
                usedSkills &= Skills.DivingTackle;
            }

            var probabilityCalculator = _calculatorFactory
                .CreateProbabilityCalculator(nextPlayerAction.Action.ActionType, nextPlayerAction.Action.NumberOfDice, this, nonCriticalFailure);

            probabilityCalculator.Calculate(p, r, nextPlayerAction, usedSkills, nonCriticalFailure);
        }

        private PlayerAction NextPlayerAction(ActionType? previousActionType, int i, PlayerAction nextPlayerAction)
        {
            return previousActionType != ActionType.Dauntless
                ? i + 1 == _context.Calculation.PlayerActions.Length ? null : _context.Calculation.PlayerActions[i + 1]
                : nextPlayerAction;
        }

        private PlayerAction GetNextPlayerAction(int i, int depth) => 
            _context.Calculation.PlayerActions.FirstOrDefault(x => x.Depth < depth && x.Index > i);

        private static bool NonCriticalFailureSupported(ActionType? previousActionType) =>
            previousActionType is ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury or ActionType.Foul 
                or ActionType.Pass or ActionType.ThrowTeamMate or ActionType.Interception;

        private static bool PlayerSentOff(ActionType? previousActionType, ActionType actionType) =>
            previousActionType switch
            {
                ActionType.Foul => actionType is not (ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury),
                ActionType.ArgueTheCall => actionType is not ActionType.Bribe,
                ActionType.Bribe => actionType is not ActionType.ArgueTheCall,
                ActionType.Injury => actionType is not (ActionType.Bribe or ActionType.ArgueTheCall),
                _ => false
            };

        private bool IsEndOfCalculation(PlayerAction playerAction) =>
            (playerAction.Index + 1 == _context.Calculation.PlayerActions.Length || playerAction.Action.TerminatesCalculation);

        private void WriteResult(decimal p, int r, Skills usedSkills, ActionType? previousActionType)
        {
            Console.WriteLine($"MaxRerolls:{_context.MaxRerolls} P:{p:0.00000} R:{r} Action:{previousActionType} UsedSkills:{usedSkills}");

            _context.Results[_context.MaxRerolls - r] += p;
        }
    }
}
