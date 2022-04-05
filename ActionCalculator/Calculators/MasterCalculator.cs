using System;
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
            if (p < 0.000001m)
            {
                return;
            }

            var i = previousPlayerAction != null ? previousPlayerAction.Index + 1 : 0;

            var previousActionType = previousPlayerAction?.Action.ActionType;
            var isEndOfCalculation = IsEndOfCalculation(previousPlayerAction);
            var previousActionIsDauntless = previousActionType == ActionType.Dauntless;

            if (nonCriticalFailure)
            {
                if (previousActionIsDauntless)
                {
                    i++;
                }

                var tentaclesFail = previousActionType == ActionType.Tentacles;
                if (tentaclesFail && isEndOfCalculation)
                {
                    return;
                }
            }
            
            if (isEndOfCalculation)
            {
                WriteResults(p, r, usedSkills, nonCriticalFailure, previousActionType);
                return;
            }
            
            var playerAction = _context.Calculation.PlayerActions[i];
            var actionType = playerAction.Action.ActionType;

            if (nonCriticalFailure)
            {
                if (PlayerSentOff(previousActionType, actionType))
                {
                    return;
                }

                if (previousActionType is not (ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury or ActionType.Foul or ActionType.Pass or ActionType.Interception)
                    && !playerAction.Action.RequiresNonCriticalFailure)
                {
                    return;
                }
            }
            else if (playerAction.Action.RequiresNonCriticalFailure)
            {
                if (previousPlayerAction?.Depth == playerAction.Depth)
                {
                    if (IsEndOfCalculation(playerAction))
                    {
                        WriteResults(p, r, usedSkills, false, previousActionType);
                        return;
                    }

                    playerAction = _context.Calculation.PlayerActions[i + 1];
                }
                else
                {
                    var depth = playerAction.Depth;
                    while (playerAction.Depth >= depth)
                    {
                        if (i >= _context.Calculation.PlayerActions.Length)
                        {
                            WriteResults(p, r, usedSkills, false, previousActionType);
                            return;
                        }

                        playerAction = _context.Calculation.PlayerActions[i];
                        i++;
                    }
                }
            }

            if (previousPlayerAction?.Player.Id != playerAction.Player.Id)
            {
                usedSkills &= Skills.DivingTackle;
            }

            var probabilityCalculator = _calculatorFactory
                .CreateProbabilityCalculator(playerAction.Action.ActionType, playerAction.Action.NumberOfDice, this, nonCriticalFailure);

            probabilityCalculator.Calculate(p, r, playerAction, usedSkills, nonCriticalFailure);            
        }

        private static bool PlayerSentOff(ActionType? previousActionType, ActionType actionType) =>
            previousActionType == ActionType.Foul && actionType is not (ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury)
            || previousActionType == ActionType.ArgueTheCall && actionType is not ActionType.Bribe
            || previousActionType == ActionType.Injury && actionType is not (ActionType.Bribe or ActionType.ArgueTheCall)
            || previousActionType == ActionType.Bribe && actionType is not ActionType.ArgueTheCall;

        private bool IsEndOfCalculation(PlayerAction playerAction)
        {
            if (playerAction == null)
            {
                return false;
            }

            return playerAction.Index + 1 >= _context.Calculation.PlayerActions.Length || playerAction.Action.TerminatesCalculation;
        }

        private void WriteResults(decimal p, int r, Skills usedSkills, bool nonCriticalFailure, ActionType? previousActionType)
        {
            Console.WriteLine($"Max Rerolls:{_context.MaxRerolls} P:{p:0.00000} R:{r} Action:{previousActionType} Non Critical Failure:{nonCriticalFailure} Used Skills:{usedSkills}");

            _context.Results[_context.MaxRerolls - r] += p;
        }
    }
}
