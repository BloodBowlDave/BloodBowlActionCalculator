using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Web.Client.Editing
{
    public class CalculationEditor(IActionTypeValidator actionTypeValidator, ICalculationBuilder calculationBuilder)
    {
        private List<Calculation> _calculations = [new Calculation(new PlayerActions(), 1)];
        private Season _season = Season.Season3;

        public IReadOnlyList<Calculation> Calculations => _calculations;

        public Calculation CurrentCalculation => _calculations[0];

        public Player CurrentPlayer { get; private set; } = new();

        public void SetSeason(Season season)
        {
            _season = season;
            CurrentCalculation.Season = season;
        }

        public void AddAction(Action action)
        {
            var player = new Player(CurrentPlayer.Id, CurrentPlayer.Skills, CurrentPlayer.LonerValue,
                CurrentPlayer.BreakTackleValue, CurrentPlayer.MightyBlowValue, CurrentPlayer.DirtyPlayerValue);

            CurrentCalculation.PlayerActions.Add(new PlayerAction(player, action, 0));
        }

        public void ChangePlayer(Player player)
        {
            CurrentPlayer = player;

            foreach (var playerAction in CurrentCalculation.PlayerActions.Where(x => x.Player.Id == player.Id))
            {
                playerAction.Player = player;
            }
        }

        public void NewPlayer()
        {
            CurrentPlayer = new Player();
        }

        public void EditPlayer(Guid playerId)
        {
            CurrentPlayer = CurrentCalculation.PlayerActions.First(x => x.Player.Id == playerId).Player;
        }

        public int PlayerNumber()
        {
            var playerIds = CurrentCalculation.PlayerActions.Select(x => x.Player.Id).Distinct().ToList();
            var playerIndex = playerIds.IndexOf(CurrentPlayer.Id);

            return playerIndex != -1 ? playerIndex + 1 : playerIds.Count + 1;
        }

        public void ClearCalculation(int index)
        {
            _calculations.RemoveAt(index);

            if (index > 0)
            {
                return;
            }

            if (_calculations.Any())
            {
                CurrentPlayer = CurrentCalculation.PlayerActions.Last().Player;
            }
            else
            {
                _calculations.Add(new Calculation(new PlayerActions(), 1, _season));
                CurrentPlayer = new Player();
            }
        }

        public void SetRerolls(int index, int rerolls)
        {
            _calculations[index].Rerolls = rerolls;
        }

        public void SaveCalculation(int index)
        {
            var calculation = _calculations[index];

            _calculations.Add(calculationBuilder.Build(calculation.ToString(), calculation.Rerolls));
        }

        public void EditCalculation(int index)
        {
            var calculations = new List<Calculation> { _calculations[index] };

            calculations.AddRange(_calculations.Where((_, i) => i != index));

            _calculations = calculations;

            CurrentPlayer = CurrentCalculation.PlayerActions.Last().Player;
        }

        public bool IsFrenzyFollowUp(int index)
        {
            var playerActions = CurrentCalculation.PlayerActions;
            var playerAction = playerActions[index];

            return playerAction.RequiresNonCriticalFailure &&
                   playerAction.Action.ActionType == ActionType.Block &&
                   index > 0 &&
                   playerActions[index - 1].Action is Block { NumberOfNonCriticalFailures: > 0 };
        }

        public void SetPushes(int index, int value)
        {
            var playerActions = CurrentCalculation.PlayerActions;
            var block = (Block)playerActions[index].Action;
            var wasUsingFrenzy = block.NumberOfNonCriticalFailures > 0;

            if (value > 0 && !wasUsingFrenzy)
            {
                block.NumberOfNonCriticalFailures = value;
                var mainPlayerAction = playerActions[index];
                var followUpBlock = new Block(block.NumberOfDice, block.NumberOfSuccessfulResults, 0, false, false, true);
                var followUpPlayerAction = new PlayerAction(mainPlayerAction.Player, followUpBlock, mainPlayerAction.Depth + 1)
                {
                    RequiresNonCriticalFailure = true,
                    EndOfBranch = true
                };
                playerActions.Insert(index + 1, followUpPlayerAction);
                return;
            }

            if (value == 0 && wasUsingFrenzy)
            {
                var nextIndex = index + 1;
                if (nextIndex < playerActions.Count && IsFrenzyFollowUp(nextIndex))
                {
                    playerActions.RemoveAt(nextIndex);
                }
            }

            block.NumberOfNonCriticalFailures = value;
        }

        public void RemoveAction(int index)
        {
            var playerActions = CurrentCalculation.PlayerActions;

            if (IsFrenzyMainBlock(index))
            {
                var followUpIndex = index + 1;
                var followUp = playerActions[followUpIndex];
                ((Block)followUp.Action).NumberOfNonCriticalFailures = 0;
                playerActions[followUpIndex] = new PlayerAction(followUp.Player, followUp.Action, 0);
            }
            else if (IsFrenzyFollowUp(index))
            {
                ((Block)playerActions[index - 1].Action).NumberOfNonCriticalFailures = 0;
            }

            playerActions.RemoveAt(index);

            RemoveInvalidPlayerActions();

            if (!CurrentCalculation.PlayerActions.Any() && _calculations.Count > 1)
            {
                _calculations.RemoveAt(0);
            }

            CurrentPlayer = CurrentCalculation.PlayerActions.LastOrDefault()?.Player ?? CurrentPlayer;
        }

        private bool IsFrenzyMainBlock(int index)
        {
            var playerActions = CurrentCalculation.PlayerActions;

            return playerActions[index].Action is Block { NumberOfNonCriticalFailures: > 0 } &&
                   index + 1 < playerActions.Count &&
                   IsFrenzyFollowUp(index + 1);
        }

        private void RemoveInvalidPlayerActions()
        {
            var playerActions = CurrentCalculation.PlayerActions;

            while (true)
            {
                var invalidIndex = -1;

                for (var i = 0; i < playerActions.Count; i++)
                {
                    var actionType = playerActions[i].Action.ActionType;
                    ActionType? previousActionType = i > 0 ? playerActions[i - 1].Action.ActionType : null;
                    ActionType? previousPreviousActionType = i > 1 ? playerActions[i - 2].Action.ActionType : null;

                    if (!actionTypeValidator.ActionTypeIsValid(actionType, previousActionType, previousPreviousActionType))
                    {
                        invalidIndex = i;
                        break;
                    }
                }

                if (invalidIndex == -1)
                {
                    return;
                }

                playerActions.RemoveAt(invalidIndex);
            }
        }
    }
}
