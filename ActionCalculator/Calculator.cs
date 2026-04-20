using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using FluentValidation;

namespace ActionCalculator
{
    public class Calculator : ICalculator
    {
        private readonly IStrategyFactory _strategyFactory;
        private readonly IValidator<Calculation> _calculationValidator;
        private readonly ICalculationContext _context;

        private Calculation _calculation;
        private decimal[] _results;

        public Calculator(IStrategyFactory strategyFactory, IValidator<Calculation> calculationValidator, ICalculationContext context)
        {
            _strategyFactory = strategyFactory;
            _calculationValidator = calculationValidator;
            _context = context;
            _calculation = null!;
            _results = null!;
        }

        public CalculationResult Calculate(Calculation calculation)
        {
            var validationResult = _calculationValidator.Validate(calculation);

            if (!validationResult.IsValid)
            {
                return new CalculationResult(validationResult.Errors);
            }

            _context.Season = calculation.Season;
            _calculation = calculation;
            _results = new decimal[calculation.Rerolls * 2 + 1];

            Resolve(1m, calculation.Rerolls, -1, CalculatorSkills.None);
            AggregateResults(_results);

            return new CalculationResult(_results.Where(x => x > 0).ToArray());
        }

        public void Resolve(decimal p, int r, int i, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            if (p > 1) {
                throw new InvalidOperationException("Probability cannot exceed one.");
            }

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

                WriteResult(p, r, usedSkills);
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
                    WriteResult(p, r, usedSkills);
                    return;
                }

                playerAction = _calculation.PlayerActions[i];
            }
            else if (IsEndOfBranch(previousPlayerAction?.BranchId, playerAction.BranchId))
            {
                i = GetNextNonBranchPlayerActionIndex(i);
                
                if (i == -1)
                {
                    WriteResult(p, r, usedSkills);
                    return;
                }

                playerAction = _calculation.PlayerActions[i];
            }

            _context.PreviousActionType = previousActionType;

            if (!IsStartOfBranch(previousPlayerAction, playerAction))
            {
                usedSkills = GetUsedSkills(previousPlayerAction?.Player.Id, player.Id, usedSkills);
                Execute(playerAction, p, r, i, usedSkills, nonCriticalFailure);
                return;
            }

            foreach (var (branchIndex, branchAction) in GetBranchStartActions(i, playerAction.BranchId))
            {
                Execute(branchAction, p, r, branchIndex, GetUsedSkills(previousPlayerAction?.Player.Id, player.Id, usedSkills), nonCriticalFailure);
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

        private IEnumerable<(int Index, PlayerAction Action)> GetBranchStartActions(int i, int branchId)
        {
            while (true)
            {
                yield return (i, _calculation.PlayerActions[i]);

                var next = -1;
                for (var j = i + 1; j < _calculation.PlayerActions.Count; j++)
                {
                    if (_calculation.PlayerActions[j].BranchId > branchId)
                    {
                        next = j;
                        break;
                    }
                }

                if (next == -1) yield break;

                i = next;
                branchId = _calculation.PlayerActions[i].BranchId;
            }
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

        private static CalculatorSkills GetUsedSkills(Guid? previousPlayerId, Guid playerId, CalculatorSkills usedSkills) =>
            previousPlayerId != playerId ? usedSkills & (CalculatorSkills.DivingTackle | CalculatorSkills.BlastIt | CalculatorSkills.CloudBurster) : usedSkills;

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

        private void Execute(PlayerAction playerAction, decimal p, int r, int i, CalculatorSkills usedSkills, bool nonCriticalFailure)
        {
            var actionStrategy = _strategyFactory.GetActionStrategy(playerAction.Action, this, nonCriticalFailure);
            actionStrategy.Execute(p, r, i, playerAction, usedSkills, nonCriticalFailure);
        }

        private void WriteResult(decimal p, int r, CalculatorSkills usedSkills)
        {
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
