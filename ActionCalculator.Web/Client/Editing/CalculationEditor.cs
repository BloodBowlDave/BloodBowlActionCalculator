using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Web.Client.Editing
{
    public class CalculationEditor(IActionTypeValidator actionTypeValidator, IPlayerActionsBuilder playerActionsBuilder)
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
            var playerCount = playerIds.Count;
            var playerIndex = playerIds.IndexOf(CurrentPlayer.Id);

            return playerIndex != -1 ? playerIndex + 1 : playerCount + 1;
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
            var playerActions = playerActionsBuilder.Build(_calculations[index].PlayerActions.ToString());

            _calculations.Add(new Calculation(playerActions, _calculations[index].Rerolls, _calculations[index].Season));
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
            var pa = CurrentCalculation.PlayerActions[index];
            return pa.RequiresNonCriticalFailure &&
                   pa.Action.ActionType == ActionType.Block &&
                   index > 0 &&
                   CurrentCalculation.PlayerActions[index - 1].Action.ActionType == ActionType.Block &&
                   ((Block)CurrentCalculation.PlayerActions[index - 1].Action).NumberOfNonCriticalFailures > 0;
        }

        public void SetPushes(int index, int value)
        {
            var block = (Block)CurrentCalculation.PlayerActions[index].Action;
            var wasUsingFrenzy = block.NumberOfNonCriticalFailures > 0;

            if (value > 0 && !wasUsingFrenzy)
            {
                block.NumberOfNonCriticalFailures = value;
                var mainPlayerAction = CurrentCalculation.PlayerActions[index];
                var followUpBlock = new Block(block.NumberOfDice, block.NumberOfSuccessfulResults, 0, false, false, true);
                var followUpPlayerAction = new PlayerAction(mainPlayerAction.Player, followUpBlock, mainPlayerAction.Depth + 1)
                {
                    RequiresNonCriticalFailure = true,
                    EndOfBranch = true
                };
                CurrentCalculation.PlayerActions.Insert(index + 1, followUpPlayerAction);
                return;
            }

            if (value == 0 && wasUsingFrenzy)
            {
                var nextIndex = index + 1;
                if (nextIndex < CurrentCalculation.PlayerActions.Count && IsFrenzyFollowUp(nextIndex))
                {
                    CurrentCalculation.PlayerActions.RemoveAt(nextIndex);
                }
            }

            block.NumberOfNonCriticalFailures = value;
        }

        public void RemoveAction(int index)
        {
            if (IsFrenzyMainBlock(index))
            {
                var followUpIndex = index + 1;
                var followUp = CurrentCalculation.PlayerActions[followUpIndex];
                ((Block)followUp.Action).NumberOfNonCriticalFailures = 0;
                CurrentCalculation.PlayerActions[followUpIndex] = new PlayerAction(followUp.Player, followUp.Action, 0);
            }
            else if (IsFrenzyFollowUp(index))
            {
                ((Block)CurrentCalculation.PlayerActions[index - 1].Action).NumberOfNonCriticalFailures = 0;
            }

            CurrentCalculation.PlayerActions.RemoveAt(index);

            RemoveInvalidPlayerActions();

            if (!CurrentCalculation.PlayerActions.Any() && _calculations.Count > 1)
            {
                _calculations.RemoveAt(0);
            }

            CurrentPlayer = CurrentCalculation.PlayerActions.Last().Player;
        }

        private bool IsFrenzyMainBlock(int index)
        {
            var pa = CurrentCalculation.PlayerActions[index];
            return pa.Action.ActionType == ActionType.Block &&
                   ((Block)pa.Action).NumberOfNonCriticalFailures > 0 &&
                   index + 1 < CurrentCalculation.PlayerActions.Count &&
                   IsFrenzyFollowUp(index + 1);
        }

        private void RemoveInvalidPlayerActions()
        {
            while (true)
            {
                var invalidPlayerActionIndexes = new List<int>();

                for (var i = 0; i < CurrentCalculation.PlayerActions.Count; i++)
                {
                    var actionType = CurrentCalculation.PlayerActions[i].Action.ActionType;
                    ActionType? previousActionType = i > 0 ? CurrentCalculation.PlayerActions[i - 1].Action.ActionType : null;
                    ActionType? previousPreviousActionType =
                        i > 1 ? CurrentCalculation.PlayerActions[i - 2].Action.ActionType : null;

                    if (!actionTypeValidator.ActionTypeIsValid(actionType, previousActionType, previousPreviousActionType))
                    {
                        invalidPlayerActionIndexes.Add(i);
                    }
                }

                foreach (var invalidPlayerActionIndex in invalidPlayerActionIndexes)
                {
                    CurrentCalculation.PlayerActions.RemoveAt(invalidPlayerActionIndex);
                }

                if (!invalidPlayerActionIndexes.Any())
                {
                    break;
                }
            }
        }
    }
}
