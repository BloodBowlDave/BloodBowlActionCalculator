using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using ActionCalculator.Web.Client.Editing;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace ActionCalculator.Web.Client.Pages
{
    public partial class Advanced
    {
        private string _calculationString = "";

        private string? _parseError;

        private readonly Dictionary<Tuple<int, string>, CalculationResult?> _resultsLookup = new();

        [Inject]
        public CalculationEditor Editor { get; set; } = null!;

        [Inject]
        public ICalculationBuilder CalculationBuilder { get; set; } = null!;

        [Inject]
        public ICalculator Calculator { get; set; } = null!;

        [Inject]
        public IValidator<Calculation> CalculationValidator { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        private IReadOnlyList<Calculation> Calculations => Editor.Calculations;

        protected override void OnInitialized()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (!QueryHelpers.ParseQuery(uri.Query).TryGetValue("c", out var calculationString))
            {
                return;
            }

            _calculationString = calculationString.ToString();
            Evaluate();
        }

        private void ClearCalculation(int index)
        {
            Editor.ClearCalculation(index);

            if (index > 0)
            {
                return;
            }

            ShowCurrentCalculation();
        }

        private void RerollsChanged(Tuple<int, int> value) => Editor.SetRerolls(value.Item1, value.Item2);

        private void SaveCalculation(int index) => Editor.SaveCalculation(index);

        private void EditCalculation(int index)
        {
            Editor.EditCalculation(index);
            ShowCurrentCalculation();
        }

        private void ShowCurrentCalculation()
        {
            _calculationString = Editor.CurrentCalculation.ToString();
            _parseError = null;
        }

        private void Evaluate()
        {
            var rerolls = Editor.CurrentCalculation.Rerolls;

            try
            {
                Editor.ReplaceCurrentCalculation(CalculationBuilder.Build(_calculationString, rerolls));
                _parseError = null;
            }
            catch (CalculationParseException e)
            {
                Editor.ReplaceCurrentCalculation(new Calculation(new PlayerActions(), rerolls));
                _parseError = e.Message;
            }
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

        private void OnKeyUp(KeyboardEventArgs obj)
        {
            if (obj.Key == "Enter")
            {
                Evaluate();
            }
        }
    }
}
