using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Web.Client.Editing;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace ActionCalculator.Web.Client.Pages
{
    public partial class Index
    {
        private readonly Dictionary<Tuple<int, string>, CalculationResult?> _resultsLookup = new();

        private bool _showPlayerEditor = true;
        private bool _showActionsContainer = true;
        private bool _showCalculation = true;

        [Inject]
        public CalculationEditor Editor { get; set; } = null!;

        [Inject]
        public ICalculator Calculator { get; set; } = null!;

        [Inject]
        public IValidator<Calculation> CalculationValidator { get; set; } = null!;

        [CascadingParameter(Name = "Season")]
        public string Season { get; set; } = "Season 3";

        private IReadOnlyList<Calculation> Calculations => Editor.Calculations;

        private Calculation CurrentCalculation() => Editor.CurrentCalculation;

        private Player CurrentPlayer => Editor.CurrentPlayer;

        protected override void OnParametersSet()
        {
            Editor.SetSeason(ParseSeason(Season));
        }

        private static Models.Season ParseSeason(string season) =>
            season == "Season 2" ? Models.Season.Season2 : Models.Season.Season3;

        private void AddAction(Models.Actions.Action action) => Editor.AddAction(action);

        private void PlayerChanged(Player player) => Editor.ChangePlayer(player);

        private void NewPlayer() => Editor.NewPlayer();

        private void EditPlayer(Guid playerId) => Editor.EditPlayer(playerId);

        private void ClearCalculation(int index) => Editor.ClearCalculation(index);

        private void RerollsChanged(Tuple<int, int> value) => Editor.SetRerolls(value.Item1, value.Item2);

        private void SaveCalculation(int index) => Editor.SaveCalculation(index);

        private void EditCalculation(int index) => Editor.EditCalculation(index);

        private int PlayerNumber() => Editor.PlayerNumber();

        private void PushesChanged(Tuple<int, int> indexAndValue) => Editor.SetPushes(indexAndValue.Item1, indexAndValue.Item2);

        private void RemoveAction(int index) => Editor.RemoveAction(index);

        private void FrenzyDiceChanged(Tuple<int, int> indexAndValue)
        {
            var (index, value) = indexAndValue;
            ((Block)CurrentCalculation().PlayerActions[index].Action).NumberOfDice = value;
        }

        private void ToggleBreakTackle(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Dodge) action).UseBreakTackle = value;
        }

        private void ToggleDivingTackle(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            if (action is Dodge dodge) dodge.UseDivingTackle = value;
            else if (action is Leap leap) leap.UseDivingTackle = value;
        }

        private void ToggleBrawler(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Block)action).UseBrawler = value;
        }

        private void ToggleHatred(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Block)action).UseHatred = value;
        }

        private void TogglePro(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            action.UsePro = value;
        }

        private void ToggleRerollInaccurate(Tuple<int, bool> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;

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
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Dauntless)action).RerollFailure = value;
        }

        private void SuccessesChanged(Tuple<int, int> indexAndValue)
        {
            var (index, value) = indexAndValue;
            var action = CurrentCalculation().PlayerActions[index].Action;
            ((Block)action).NumberOfSuccessfulResults = value;
        }

        private CalculationResult? GetCalculationResult(Calculation calculation)
        {
            var key = new Tuple<int, string>(calculation.Rerolls, calculation.ToString());

            if (_resultsLookup.TryGetValue(key, out var result))
            {
                return result;
            }

            try
            {
                result = Calculator.Calculate(calculation);
            }
            catch (InvalidCalculationResultException e)
            {
                Console.WriteLine(e.Message);
                result = null;
            }

            _resultsLookup.Add(key, result);
            return result;
        }

        private bool HasInvalidResult(Calculation calculation) => GetCalculationResult(calculation) == null;

        private IEnumerable<Tuple<int, decimal>> GetResults(Calculation calculation)
        {
            var results = GetCalculationResult(calculation)?.Results ?? [];

            for (var i = 0; i < results.Length; i++)
            {
                yield return new Tuple<int, decimal>(i, results[i]);
            }
        }
    }
}
