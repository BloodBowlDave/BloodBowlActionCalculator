using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ActionBuilders;

public class InterceptionActionBuilder : IActionBuilder
{
    public Action Build(string input)
    {
        throw new NotImplementedException();
    }
}