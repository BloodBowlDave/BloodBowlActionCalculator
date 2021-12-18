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
            var actionIndex = 0;

            for (var playerIndex = 0; playerIndex < playerStrings.Length; playerIndex++)
            {
	            var playerString = playerStrings[playerIndex];
	            var playerSplit = playerString.Split(':');

	            var player = playerSplit.Length > 1
		            ? _playerParser.Parse(playerSplit[1], playerIndex)
		            : new Player(playerIndex, Skills.None, null, null);
                
	            var actions = playerSplit[0].Split(',')
		            .Select(x => _actionBuilder.Build(x));

	            foreach (var action in actions)
	            {
		            playerActions.Add(new PlayerAction(player, action, actionIndex));
		            actionIndex++;
	            }
            }

            return new Calculation(playerActions.ToArray());
        }
    }
}
