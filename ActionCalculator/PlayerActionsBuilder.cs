using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator
{
    public class PlayerActionsBuilder : IPlayerActionsBuilder
    {
        private readonly IActionParserFactory _actionParserFactory;
        private readonly IPlayerBuilder _playerBuilder;
        private List<PlayerAction> _playerActions = new();

        public PlayerActionsBuilder(IActionParserFactory actionParserFactory, IPlayerBuilder playerBuilder)
        {
            _actionParserFactory = actionParserFactory;
            _playerBuilder = playerBuilder;
        }

        public PlayerActions Build(string playerActionsString)
        {
            _playerActions = new List<PlayerAction>();

            try
            {
                BuildPlayerActions(playerActionsString, new Player(), 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new PlayerActions();
            }

            var playerActions = new PlayerActions();
            playerActions.AddRange(_playerActions);

            return playerActions;
        }
        
        private void BuildPlayerActions(string calculation, Player player, int depth)
        {
            var (specialCharacter, index) = GetFirstSpecialCharacter(calculation);

            switch (specialCharacter)
            {
                case ':':
                    player = _playerBuilder.Build(calculation[..index]);
                    BuildPlayerActions(calculation[(index + 1)..], player, depth);
                    break;
                case '{':
                    BuildPlayerActionsWithBranch(calculation, player, depth, index, calculation.LastIndexOf('}'), false);
                    break;
                case '[':
                    BuildPlayerActionsWithBranch(calculation, player, depth, index, calculation.LastIndexOf(']'), true);
                    break;
                case '(':
                    BuildPlayerActionsWithAlternateBranches(calculation, player, depth, index, calculation.LastIndexOf(')'));
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
                foreach (var specialCharacter in new List<char> { ':', ';', '{', '[', '(' }.Where(x => character == x))
                {
                    return new Tuple<char?, int>(specialCharacter, i);
                }
            }

            return new Tuple<char?, int>(null, -1);
        }

        private void BuildPlayerActionsForOnePlayer(string calculation, Player player, int depth)
        {
            foreach (var action in GetActions(calculation))
            {
                AddPlayerAction(player, action, depth);
            }
        }

        private void BuildPlayerActionsForMultiplePlayers(string calculation, Player player, int depth, int indexOfPlayerEnd)
        {
            foreach (var action in GetActions(calculation[..indexOfPlayerEnd]))
            {
                AddPlayerAction(player, action, depth);
            }

            BuildPlayerActions(calculation[(indexOfPlayerEnd + 1)..], new Player(), depth);
        }

        private void BuildPlayerActionsWithBranch(string calculation, Player player, int depth, int indexOfOpeningBracket, int lastIndexOfClosingBracket, bool terminatesCalculation)
        {
            if (lastIndexOfClosingBracket == -1)
            {
                throw new Exception("No matching closing bracket.");
            }
            
            foreach (var action in GetActions(calculation[..indexOfOpeningBracket]))
            {
                AddPlayerAction(player, action, depth);
            }

            var calculationLength = _playerActions.Count;

            BuildPlayerActions(calculation[(indexOfOpeningBracket + 1)..lastIndexOfClosingBracket], player, depth + 1);

            var branchLength = _playerActions.Count - calculationLength;

            if (branchLength > 0)
            {
                _playerActions[calculationLength].RequiresNonCriticalFailure = true;
                _playerActions[calculationLength + branchLength - 1].EndOfBranch = true;

                if (terminatesCalculation)
                {
                    _playerActions[calculationLength + branchLength - 1].TerminatesCalculation = true;
                }
            }

            foreach (var action in GetActions(calculation[(lastIndexOfClosingBracket + 1)..]))
            {
                AddPlayerAction(player, action, depth);
            }
        }

        private void BuildPlayerActionsWithAlternateBranches(string calculation, Player player, int depth, int indexOfOpeningBracket, int lastIndexOfClosingBracket)
        {
            if (lastIndexOfClosingBracket == -1)
            {
                throw new Exception("No matching closing bracket.");
            }
            
            foreach (var action in GetActions(calculation[..indexOfOpeningBracket]))
            {
                AddPlayerAction(player, action, depth);
            }

            var branches = calculation[(indexOfOpeningBracket + 1)..lastIndexOfClosingBracket].Split(")(");

            for (var i = 0; i < branches.Length; i++)
            {
                var calculationLength = _playerActions.Count;

                BuildPlayerActions(branches[i], player, depth + 1);

                for (var j = calculationLength; j < _playerActions.Count; j++)
                {
                    _playerActions[j].BranchId = i + 1;
                }
            }

            foreach (var action in GetActions(calculation[(lastIndexOfClosingBracket + 1)..]))
            {
                AddPlayerAction(player, action, depth);
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

                yield return _actionParserFactory.GetActionParser(actionSplit[0]).Parse(actionSplit[0]);

                if (actionSplit.Length == 1)
                {
                    continue;
                }
                
                yield return _actionParserFactory.GetActionParser(actionSplit[1]).Parse(actionSplit[1]);
            }
        }

        private void AddPlayerAction(Player player, Action action, int depth)
        {
            _playerActions.Add(new PlayerAction(player, action, depth));
        }
    }
}
