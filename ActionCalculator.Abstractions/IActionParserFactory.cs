using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface IActionParserFactory
    {
        IActionParser GetActionParser(string input);
        IActionParser GetActionParser(ActionType actionType);
    }
}