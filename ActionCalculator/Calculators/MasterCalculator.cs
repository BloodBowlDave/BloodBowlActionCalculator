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
            var dauntlessFail = previousActionType == ActionType.Dauntless && nonCriticalFailure;
            if (dauntlessFail)
            {
                i++;
            }
            
            if (i >= _context.Calculation.PlayerActions.Length)
            {
                WriteResults(p, r, usedSkills, nonCriticalFailure, previousActionType);
                return;
            }
            
            var playerAction = _context.Calculation.PlayerActions[i];
            var actionType = playerAction.Action.ActionType;

            if (nonCriticalFailure)
            {
                switch (previousActionType)
                {
                    case ActionType.Foul when actionType is not (ActionType.Bribe or ActionType.ArgueTheCall or ActionType.Injury):
                    case ActionType.ArgueTheCall when actionType is not ActionType.Bribe:
                    case ActionType.Injury when actionType is not (ActionType.Bribe or ActionType.ArgueTheCall):
                    case ActionType.Bribe when actionType is not ActionType.ArgueTheCall:
                        return;
                }
            }

            if (playerAction.Action.RequiresDauntlessFail && !dauntlessFail)
            {
                i++;

                if (i >= _context.Calculation.PlayerActions.Length)
                {
                    WriteResults(p, r, usedSkills, nonCriticalFailure, previousActionType);
                    return;
                }

                playerAction = _context.Calculation.PlayerActions[i];
            }

            if (previousPlayerAction?.Player.Index != playerAction.Player.Index)
            {
                usedSkills &= Skills.DivingTackle;
            }

            var probabilityCalculator = _calculatorFactory
                .CreateProbabilityCalculator(actionType, playerAction.Action.NumberOfDice, this, nonCriticalFailure);

            probabilityCalculator.Calculate(p, r, playerAction, usedSkills, nonCriticalFailure);            
        }

        private void WriteResults(decimal p, int r, Skills usedSkills, bool nonCriticalFailure, ActionType? previousActionType)
        {
            Console.WriteLine($"Max Rerolls:{_context.MaxRerolls} P:{p:0.00000} R:{r} Action:{previousActionType} Non Critical Failure:{nonCriticalFailure} Used Skills:{usedSkills}");

            _context.Results[_context.MaxRerolls - r] += p;
        }
    }
}
