using ActionCalculator.Abstractions;
using ActionCalculator.Models;

namespace ActionCalculator
{
    public class CalculationContext : ICalculationContext
    {
        public Season Season { get; set; } = Season.Season3;
        public ActionType? PreviousActionType { get; set; }
    }
}
