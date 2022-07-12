using ActionCalculator.Models;

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
            _calculation.PlayerActions.Add(new PlayerAction(_currentPlayer, action, 0));
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

        private void RemoveAction(int i)
        {
            _calculation.PlayerActions.RemoveAt(i);
        }
    }
}
