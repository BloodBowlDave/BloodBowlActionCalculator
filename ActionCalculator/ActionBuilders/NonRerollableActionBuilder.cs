using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ActionBuilders;

public class NonRerollableActionBuilder : IActionBuilder
{
    public Action Build(string input)
    {
        throw new NotImplementedException();
    }
}