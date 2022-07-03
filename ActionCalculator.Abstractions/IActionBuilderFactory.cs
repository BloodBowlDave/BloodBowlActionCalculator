using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface IActionBuilderFactory
    {
        IActionBuilder GetActionBuilder(string input);
        IActionBuilder GetActionBuilder(ActionType actionType);
    }
}