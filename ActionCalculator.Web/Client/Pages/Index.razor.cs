using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace ActionCalculator.Web.Client.Pages
{
    public partial class Index
    {
        private List<Calculation> _calculations = new();

        private readonly Dictionary<Tuple<int, string>, CalculationResult> _resultsLookup = new();

        [Inject] 
        public IPlayerActionsBuilder PlayerActionsBuilder { get; set; } = null!;

        [Inject]
        public ICalculator Calculator { get; set; } = null!;

        [Inject]
        public IActionTypeValidator ActionTypeValidator { get; set; } = null!;

        [Inject]
        public IValidator<Calculation> CalculationValidator { get; set; } = null!;

        private Calculation CurrentCalculation() => _calculations.First();

        private Player CurrentPlayer { get; set; }
        
        public Index()
        {
            _calculations.Add(new Calculation(new PlayerActions(), 1));
            CurrentPlayer = new Player();
        }

        private void AddAction(Models.Actions.Action action)
        {
            var player = new Player(CurrentPlayer.Id, CurrentPlayer.Skills, CurrentPlayer.LonerValue,
                CurrentPlayer.BreakTackleValue, CurrentPlayer.MightyBlowValue, CurrentPlayer.DirtyPlayerValue, 0);

            CurrentCalculation().PlayerActions.Add(new PlayerAction(player, action, 0));
        }

        private void PlayerChanged(Player player)
        {
            CurrentPlayer = player;

            foreach (var playerAction in CurrentCalculation().PlayerActions.Where(x => x.Player.Id == player.Id))
            {
                playerAction.Player = player;
            }
        }

        private void NewPlayer()
        {
            CurrentPlayer = new Player();
        }

        private void EditPlayer(Guid playerId)
        {
            CurrentPlayer = CurrentCalculation().PlayerActions.First(x => x.Player.Id == playerId).Player;
        }

        private void ClearCalculation(int index)
        {
            _calculations.RemoveAt(index);

            if (index > 0)
            {
                return;
            }

            if (_calculations.Any())
            {
                CurrentPlayer = CurrentCalculation().PlayerActions.Last().Player;
            }
            else
            {
                _calculations.Add(new Calculation(new PlayerActions(), 1));
                CurrentPlayer = new Player();
            }
        }

        private void RerollsChanged(Tuple<int, int> value)
        {
            var (index, rerolls) = value;

            _calculations[index].Rerolls = rerolls;
        }

        private void SaveCalculation(int index)
        {
            var playerActions = PlayerActionsBuilder.Build(_calculations[index].PlayerActions.ToString());

            _calculations.Add(new Calculation(playerActions, _calculations[index].Rerolls));
        }

        private void EditCalculation(int index)
        {
            var calculations = new List<Calculation> { _calculations[index] };

            calculations.AddRange(_calculations.Where((_, i) => i != index));

            _calculations = calculations;

            CurrentPlayer = CurrentCalculation().PlayerActions.Last().Player;
        }

        private int PlayerNumber()
        {
            var playerIds = CurrentCalculation().PlayerActions.Select(x => x.Player.Id).Distinct().ToList();
            var playerCount = playerIds.Count;
            var playerIndex = playerIds.IndexOf(CurrentPlayer.Id);
            
            return playerIndex != -1 ? playerIndex + 1 : playerCount + 1;
        }

        private void RemoveAction(int index)
        {
            CurrentCalculation().PlayerActions.RemoveAt(index);
            
            RemoveInvalidPlayerActions();
            
            if (!CurrentCalculation().PlayerActions.Any() && _calculations.Count > 1)
            {
                _calculations.RemoveAt(0);
            }

            CurrentPlayer = CurrentCalculation().PlayerActions.Last().Player;
        }

        private void RemoveInvalidPlayerActions()
        {
            while (true)
            {
                var invalidPlayerActionIndexes = new List<int>();

                for (var i = 0; i < CurrentCalculation().PlayerActions.Count; i++)
                {
                    var actionType = CurrentCalculation().PlayerActions[i].Action.ActionType;
                    ActionType? previousActionType = i > 0 ? CurrentCalculation().PlayerActions[i - 1].Action.ActionType : null;
                    ActionType? previousPreviousActionType =
                        i > 1 ? CurrentCalculation().PlayerActions[i - 2].Action.ActionType : null;

                    if (!ActionTypeValidator.ActionTypeIsValid(actionType, previousActionType, previousPreviousActionType))
                    {
                        invalidPlayerActionIndexes.Add(i);
                    }
                }

                foreach (var invalidPlayerActionIndex in invalidPlayerActionIndexes)
                {
                    CurrentCalculation().PlayerActions.RemoveAt(invalidPlayerActionIndex);
                }

                if (!invalidPlayerActionIndexes.Any())
                {
                    break;
                }
            }
        }

        private void ToggleBreakTackle(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Dodge) action).UseBreakTackle = value;
        }

        private void ToggleDivingTackle(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Dodge)action).UseDivingTackle = value;
        }

        private void ToggleBrawler(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Block)action).UseBrawler = value;
        }

        private void TogglePro(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            action.UsePro = value;
        }

        private void ToggleRerollInaccurate(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;

            switch (action.ActionType)
            {
                case ActionType.Pass:
                    ((Pass) action).RerollInaccuratePass = value;
                    break;
                case ActionType.ThrowTeammate:
                    ((ThrowTeammate)action).RerollInaccurateThrow = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ToggleRerollFailure(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Dauntless)action).RerollFailure = value;
        }

        private void SuccessesChanged(Tuple<int, int> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Block)action).NumberOfSuccessfulResults = value;
        }

        private IEnumerable<Tuple<int, decimal>> GetResults(Calculation calculation)
        {
            var key = new Tuple<int, string>(calculation.Rerolls, calculation.PlayerActions.ToString());
            CalculationResult result;

            if (_resultsLookup.ContainsKey(key))
            {
                result = _resultsLookup[key];
            }
            else
            {
                result = Calculator.Calculate(calculation);

                _resultsLookup.Add(key, result);
            }

            for (var i = 0; i < result.Results.Length; i++)
            {
                yield return new Tuple<int, decimal>(i, result.Results[i]);
            }
        }
    }
}
