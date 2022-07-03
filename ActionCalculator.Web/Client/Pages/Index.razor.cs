using ActionCalculator.Models;

namespace ActionCalculator.Web.Client.Pages
{
    public partial class Index
    {
        private Calculation _calculation;
        private const int MaxRerolls = 0;
        private Player _currentPlayer = new();

        public Index()
        {
            _calculation = new Calculation(new PlayerActions(), MaxRerolls);
        }
        
        private void AddAction(Models.Actions.Action action)
        {
            _calculation.PlayerActions.Add(new PlayerAction(_currentPlayer, action, 0));
        }

        private void OnPlayerChanged(Player player)
        {
            _currentPlayer = player;
        }
    }
}
