using System.Linq;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class CalculationBuilder
    {
        private readonly IActionBuilder _actionBuilder;
        private readonly IPlayerParser _playerParser;

        public CalculationBuilder(IActionBuilder actionBuilder, IPlayerParser playerParser)
        {
            _actionBuilder = actionBuilder;
            _playerParser = playerParser;
        }

        public Calculation Build(string calculation)
        {
            var playerStrings = calculation.Split('(', ')').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var players = new Player[playerStrings.Length];

            for (var i = 0; i < playerStrings.Length; i++)
            {
                var playerString = playerStrings[i];
                var playerSplit = playerString.Split(':');
                
                var player = playerSplit.Length > 1 ? _playerParser.Parse(playerSplit[1]) : new Player();
                
                var actions = playerSplit[0].Split(',').Select(x => _actionBuilder.Build(x, player.Skills)).ToArray();
                player.Actions = actions;

                players[i] = player;
            }

            return new Calculation(players);
        }
    }
}
