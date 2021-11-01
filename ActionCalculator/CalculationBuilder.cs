using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class CalculationBuilder : ICalculationBuilder
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
            var playerStrings = calculation.Split('(', ')')
	            .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var playerActions = new List<PlayerAction>();

            for (var i = 0; i < playerStrings.Length; i++)
            {
	            var playerString = playerStrings[i];
	            var playerSplit = playerString.Split(':');

	            var player = playerSplit.Length > 1
		            ? _playerParser.Parse(playerSplit[1], i)
		            : new Player(i);

	            var actions = playerSplit[0].Split(',')
		            .Select(x => _actionBuilder.Build(x)).ToArray();

                playerActions.AddRange(actions.Select((x, j) 
                    => new PlayerAction(player, x, j)));
            }

            return new Calculation(playerActions.ToArray());
        }
    }
}
