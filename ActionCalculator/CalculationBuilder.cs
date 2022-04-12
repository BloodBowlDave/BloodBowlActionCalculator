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
        private List<PlayerAction> _playerActions;

        public CalculationBuilder(IActionBuilder actionBuilder, IPlayerParser playerParser)
        {
            _actionBuilder = actionBuilder;
            _playerParser = playerParser;
        }

        public Calculation Build(string calculation)
        {
            _playerActions = new List<PlayerAction>();

            BuildPlayerActions(calculation, new Player(), 0);
            
            return new Calculation(_playerActions.ToArray());
        }

        private void AddPlayerActions(IEnumerable<PlayerAction> playerActions)
        {
            foreach (var playerAction in playerActions)
            {
                playerAction.Index = _playerActions.Count;
                _playerActions.Add(playerAction);
            }
        }

        private void BuildPlayerActions(string calculation, Player player, int depth)
        {
            var (specialCharacter, index) = GetFirstSpecialCharacter(calculation);

            switch (specialCharacter)
            {
                case ':':
                    player = _playerParser.Parse(calculation[..index]);
                    BuildPlayerActions(calculation[(index + 1)..], player, depth);
                    break;
                case '{':
                    BuildPlayerActionsWithBranch(calculation, player, depth, index);
                    break;
                case '[':
                    BuildPlayerActionsWithFork(calculation, player, depth, index);
                    break;
                case ';':
                    BuildPlayerActionsForMultiplePlayers(calculation, player, depth, index);
                    break;
                default:
                    BuildPlayerActionsForOnePlayer(calculation, player, depth);
                    break;
            }
        }

        private static Tuple<char?, int> GetFirstSpecialCharacter(string calculation)
        {
            for (var i = 0; i < calculation.Length; i++)
            {
                var character = calculation[i];
                foreach (var specialCharacter in new List<char> {':', ';', '{', '[', '('}.Where(x => character == x))
                {
                    return new Tuple<char?, int>(specialCharacter, i);
                }
            }

            return new Tuple<char?, int>(null, -1);
        }

        private void BuildPlayerActionsForOnePlayer(string calculation, Player player, int depth) =>
            AddPlayerActions(GetActions(calculation).Select(x => new PlayerAction(player, x, depth)));
        
        private void BuildPlayerActionsForMultiplePlayers(string calculation, Player player, int depth, int indexOfPlayerEnd)
        {
            AddPlayerActions(GetActions(calculation[..indexOfPlayerEnd]).Select(x => new PlayerAction(player, x, depth)));
            BuildPlayerActions(calculation[(indexOfPlayerEnd + 1)..], new Player(), depth);
        }

        private void BuildPlayerActionsWithFork(string calculation, Player player, int depth, int indexOfOpeningSquareBracket)
        {
            var indexOfClosingSquareBracket = calculation.LastIndexOf(']');

            if (indexOfClosingSquareBracket == -1)
            {
                throw new Exception("No matching closing bracket.");
            }

            AddPlayerActions(GetActions(calculation[..indexOfOpeningSquareBracket]).Select(x => new PlayerAction(player, x, depth)));
            
            var calculationLength = _playerActions.Count;

            BuildPlayerActions(calculation[(indexOfOpeningSquareBracket + 1)..indexOfClosingSquareBracket], player, depth + 1);

            var lengthOfFork = _playerActions.Count - calculationLength;

            if (lengthOfFork > 0)
            {
                _playerActions[calculationLength].Action.RequiresNonCriticalFailure = true;
                _playerActions[calculationLength + lengthOfFork - 1].Action.TerminatesCalculation = true;
            }

            AddPlayerActions(GetActions(calculation[(indexOfClosingSquareBracket + 1)..]).Select(x => new PlayerAction(player, x, depth)));
        }

        private void BuildPlayerActionsWithBranch(string calculation, Player player, int depth, int indexOfOpeningBrace)
        {
            var indexOfClosingBrace = calculation.LastIndexOf('}');

            if (indexOfClosingBrace == -1)
            {
                throw new Exception("No matching closing brace.");
            }

            AddPlayerActions(GetActions(calculation[..indexOfOpeningBrace]).Select(x => new PlayerAction(player, x, depth)));

            var calculationLength = _playerActions.Count;

            BuildPlayerActions(calculation[(indexOfOpeningBrace + 1)..indexOfClosingBrace], player, depth + 1);
            
            if (_playerActions.Count - calculationLength > 0)
            {
                _playerActions[calculationLength].Action.RequiresNonCriticalFailure = true;
            }

            AddPlayerActions(GetActions(calculation[(indexOfClosingBrace + 1)..]).Select(x => new PlayerAction(player, x, depth)));
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
