using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace ActionCalculator.Web.Client.Pages
{
    public partial class Advanced
    {
        private string _calculationString;

        private string? _parseError;
        
        private List<Calculation> _calculations = new();

        private readonly Dictionary<Tuple<int, string>, CalculationResult?> _resultsLookup = new();

        [Inject] 
        public IPlayerActionsBuilder PlayerActionsBuilder { get; set; } = null!;

        [Inject]
        public ICalculator Calculator { get; set; } = null!;
        
        [Inject]
        public IValidator<Calculation> CalculationValidator { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;
        
        public Advanced()
        {
            _calculations.Add(new Calculation(new PlayerActions(), 1));
            _calculationString = "";
        }
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
            _calculations.RemoveAt(index);

            if (index > 0)
            {
                return;
            }
            
            if (!_calculations.Any())
            {
                _calculations.Add(new Calculation(new PlayerActions(), 1));
            }

            _calculationString = _calculations[0].PlayerActions.ToString();
            _parseError = null;
        }

        private void RerollsChanged(Tuple<int, int> value)
        {
            var (index, rerolls) = value;

            _calculations[index].Rerolls = rerolls;
        }

        private void SaveCalculation(int index)
        {
            var playerActions = PlayerActionsBuilder.Build(_calculations[index].PlayerActions.ToString());

            _calculations.Add(new Calculation(playerActions, _calculations[index].Rerolls));
        }

        private void EditCalculation(int index)
        {
            var calculations = new List<Calculation> { _calculations[index] };

            calculations.AddRange(_calculations.Where((_, i) => i != index));

            _calculations = calculations;

            _calculationString = _calculations[0].PlayerActions.ToString();
            _parseError = null;
        }

        private void Evaluate()
        {
            try
            {
                _calculations[0] = new Calculation(PlayerActionsBuilder.Build(_calculationString), 1);
                _parseError = null;
            }
            catch (CalculationParseException e)
            {
                _calculations[0] = new Calculation(new PlayerActions(), 1);
                _parseError = e.Message;
            }
        }
        
        private CalculationResult? GetCalculationResult(Calculation calculation)
        {
            var key = new Tuple<int, string>(calculation.Rerolls, calculation.PlayerActions.ToString());

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
