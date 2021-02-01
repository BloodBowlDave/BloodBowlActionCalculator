using System.Linq;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class CalculationBuilder
    {
        private readonly IActionParser _actionParser;
        private readonly IPlayerParser _playerParser;

        public CalculationBuilder(IActionParser actionParser, IPlayerParser playerParser)
        {
            _actionParser = actionParser;
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

                var actions = playerSplit[0].Split(',').Select(x => _actionParser.Parse(x)).ToArray();

                var player = playerSplit.Length > 1 ? _playerParser.Parse(playerSplit[1]) : new Player();

                player.Actions = actions;

                players[i] = player;
            }

            return new Calculation(players);
        }
    }
}
