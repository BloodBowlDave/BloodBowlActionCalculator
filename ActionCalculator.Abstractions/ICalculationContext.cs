using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface ICalculationContext
    {
        Season Season { get; set; }
        ActionType? PreviousActionType { get; set; }
    }
}
