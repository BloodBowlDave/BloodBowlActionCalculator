using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Web.Client.Pages
{
    public partial class Index
    {
        private Calculation _calculation;
        private Player _currentPlayer = new();

        public Index()
        {
            _calculation = new Calculation(new PlayerActions(), 1);
        }
        
        private void AddAction(Models.Actions.Action action)
        {
            var index = _calculation.PlayerActions.LastOrDefault()?.Index + 1 ?? 0;
            _calculation.PlayerActions.Add(new PlayerAction(_currentPlayer, action, 0, index));
        }

        private void PlayerChanged(Player player)
        {
            foreach (var playerAction in _calculation.PlayerActions.Where(x => x.Player.Id == player.Id))
            {
                playerAction.Player = player;
            }

            _currentPlayer = player;
        }

        private void CalculationChanged(Calculation calculation)
        {
            _calculation = calculation;
        }

        private int PlayerNumber()
        {
            var playerIds = _calculation.PlayerActions.Select(x => x.Player.Id).Distinct().ToList();
            var playerCount = playerIds.Count;

            return playerIds.Contains(_currentPlayer.Id) ? playerCount : playerCount + 1;
        }

        private void RemoveAction(int index)
        {
            for (var i = 0; i < _calculation.PlayerActions.Count; i++)
            {
                if (_calculation.PlayerActions[i].Index == index)
                {
                    _calculation.PlayerActions.RemoveAt(i);
                }
            }
        }

        private void ToggleBreakTackle(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = _calculation.PlayerActions.Single(x => x.Index == index).Action;
            ((Dodge) action).UseBreakTackle = value;
        }

        private void ToggleDivingTackle(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = _calculation.PlayerActions.Single(x => x.Index == index).Action;
            ((Dodge)action).UseDivingTackle = value;
        }

        private void ToggleBrawler(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = _calculation.PlayerActions.Single(x => x.Index == index).Action;
            ((Block)action).UseBrawler = value;
        }

        private void TogglePro(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = _calculation.PlayerActions.Single(x => x.Index == index).Action;
            action.UsePro = value;
        }

        private void ToggleRerollInaccurate(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = _calculation.PlayerActions.Single(x => x.Index == index).Action;

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
            var action = _calculation.PlayerActions.Single(x => x.Index == index).Action;
            ((Dauntless)action).RerollFailure = value;
        }

        private bool DisableAllButBlock()
        {
            var playerActionCount = _calculation.PlayerActions.Count;
            var lastActionIsDauntless = _calculation.PlayerActions.LastOrDefault()?.Action.ActionType == ActionType.Dauntless;
            var penultimateActionIsDauntless = playerActionCount > 1 && _calculation.PlayerActions[playerActionCount - 2].Action.ActionType == ActionType.Dauntless;

            return lastActionIsDauntless || penultimateActionIsDauntless;
        }
    }
}
