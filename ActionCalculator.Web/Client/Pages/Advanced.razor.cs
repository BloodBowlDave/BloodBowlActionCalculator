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
        
        private List<Calculation> _calculations = new();

        private readonly Dictionary<Tuple<int, string>, CalculationResult> _resultsLookup = new();

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

            _calculationString = calculationString;
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
        }

        private void Evaluate()
        {
            var playerActions = PlayerActionsBuilder.Build(_calculationString);
            
            _calculations[0] = new Calculation(playerActions, 1);
        }
        
        private IEnumerable<Tuple<int, decimal>> GetResults(Calculation calculation)
        {
            var key = new Tuple<int, string>(calculation.Rerolls, calculation.PlayerActions.ToString());
            CalculationResult result;

            if (_resultsLookup.ContainsKey(key))
            {
                result = _resultsLookup[key];
            }
            else
            {
                result = Calculator.Calculate(calculation);

                _resultsLookup.Add(key, result);
            }

            for (var i = 0; i < result.Results.Length; i++)
            {
                yield return new Tuple<int, decimal>(i, result.Results[i]);
            }
        }

        private bool CalculationIsValid()
        {
            if (string.IsNullOrWhiteSpace(_calculationString))
            {
                return true;
            }

            var playerActions = _calculations[0].PlayerActions;

            return playerActions.Any();
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
