using System;
using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

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
            var playerActions = new List<PlayerAction>();
            var startIndex = 0;
            var i = 0;

            //while (startIndex < calculation.Length)
            //{
            //    Player player;
            //    var indexOfSkillsStart = calculation[startIndex..].IndexOf(':');
            //    int indexOfPlayerEnd;

            //    if (indexOfSkillsStart == -1)
            //    {
            //        player = new Player();
            //        indexOfPlayerEnd = calculation.Length;
            //    }
            //    else
            //    {
            //        var indexOfSkillsEnd = calculation[indexOfSkillsStart..].IndexOf(',');
            //        indexOfPlayerEnd = indexOfSkillsEnd == -1 ? calculation.Length : startIndex + indexOfSkillsStart + indexOfSkillsEnd;
            //        player = _playerParser.Parse(calculation[(indexOfSkillsStart + 1)..indexOfPlayerEnd]);
            //    }

            //    foreach (var playerAction in GetPlayerActions(calculation[startIndex..indexOfPlayerEnd], player))
            //    {
            //        playerAction.Index = i;
            //        i++;
            //        playerActions.Add(playerAction);
            //    }

            //    startIndex = indexOfPlayerEnd + 1;
            //}

            var playerStrings = calculation.Split('(', ')')
                .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            foreach (var playerString in playerStrings)
            {
                var playerSplit = playerString.Split(':');

                var player = playerSplit.Length > 1
                    ? _playerParser.Parse(playerSplit[1])
                    : new Player();

                foreach (var playerAction in GetPlayerActions(playerString, player))
                {
                    playerAction.Index = i;
                    i++;
                    playerActions.Add(playerAction);
                }
            }

            return new Calculation(playerActions.ToArray());
        }

        private IEnumerable<PlayerAction> GetPlayerActions(string playerString, Player player, int depth = 0)
        {
            var indexOfOpeningBrace = playerString.IndexOf('{');
            var indexOfOpeningSquareBracket = playerString.IndexOf('[');

            if (indexOfOpeningBrace > -1 && (indexOfOpeningBrace < indexOfOpeningSquareBracket || indexOfOpeningSquareBracket == -1))
            {
                var indexOfClosingBrace = playerString.LastIndexOf('}');

                if (indexOfClosingBrace == -1)
                {
                    throw new Exception("No matching closing brace.");
                }

                var indexOfColon = playerString.LastIndexOf(':');
                if (indexOfColon > indexOfClosingBrace)
                {
                    player = _playerParser.Parse(playerString[(indexOfColon + 1)..]);
                }
                
                foreach (var playerAction in GetActions(playerString[..indexOfOpeningBrace]).Select(x => new PlayerAction(player, x, depth)))
                {
                    yield return playerAction;
                }

                var isFirstAction = true;
                foreach (var playerAction in GetPlayerActions(playerString[(indexOfOpeningBrace + 1)..indexOfClosingBrace], player, depth + 1))
                {
                    if (isFirstAction)
                    {
                        playerAction.Action.RequiresNonCriticalFailure = true;
                        isFirstAction = false;
                    }

                    yield return playerAction;
                }

                var endOfActionString = indexOfColon != -1 ? indexOfColon : playerString.Length;

                foreach (var playerAction in GetActions(playerString[(indexOfClosingBrace + 1)..endOfActionString]).Select(x => new PlayerAction(player, x, depth)))
                {
                    yield return playerAction;
                }
            }
            else if (indexOfOpeningSquareBracket > -1)
            {
                var indexOfClosingSquareBracket = playerString.LastIndexOf(']');

                if (indexOfClosingSquareBracket == -1)
                {
                    throw new Exception("No matching closing bracket.");
                }

                var indexOfColon = playerString.LastIndexOf(':');
                if (indexOfColon > indexOfClosingSquareBracket)
                {
                    player = _playerParser.Parse(playerString[(indexOfColon + 1)..]);
                }

                foreach (var playerAction in GetActions(playerString[..indexOfOpeningSquareBracket]).Select(x => new PlayerAction(player, x, depth)))
                {
                    yield return playerAction;
                }

                var isFirstAction = true;
                PlayerAction previousPlayerAction = null;

                foreach (var playerAction in GetPlayerActions(playerString[(indexOfOpeningSquareBracket + 1)..indexOfClosingSquareBracket], new Player(), depth + 1))
                {
                    if (previousPlayerAction != null)
                    {
                        yield return previousPlayerAction;
                    }

                    if (isFirstAction)
                    {
                        playerAction.Action.RequiresNonCriticalFailure = true;
                        isFirstAction = false;
                    }

                    previousPlayerAction = playerAction;
                }

                if (previousPlayerAction != null)
                {
                    previousPlayerAction.Action.TerminatesCalculation = true;
                    yield return previousPlayerAction;
                }

                var endOfActionString = indexOfColon != -1 ? indexOfColon : playerString.Length;

                foreach (var playerAction in GetActions(playerString[(indexOfClosingSquareBracket + 1)..endOfActionString]).Select(x => new PlayerAction(player, x, depth)))
                {
                    yield return playerAction;
                }
            }
            else
            {
                var playerSplit = playerString.Split(':');

                if (playerSplit.Length > 1)
                {
                    player = _playerParser.Parse(playerSplit[1]);
                }
                
                foreach (var playerAction in GetActions(playerSplit[0]).Select(x => new PlayerAction(player, x, depth)))
                {
                    yield return playerAction;
                }
            }
        }

        private IEnumerable<Action> GetActions(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                yield break;
            }

            foreach (var actionString in input.Split(','))
            {
                var actionSplit = actionString.Split('|');

                yield return _actionBuilder.Build(actionSplit[0]);

                if (actionSplit.Length == 1)
                {
                    continue;
                }

                var action = _actionBuilder.Build(actionSplit[1]);

                action.RequiresNonCriticalFailure = true;

                yield return action;
            }
        }
    }
}
