using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ActionBuilders;

public class BribeActionBuilder : IActionBuilder
{
    public Action Build(string input) => new Bribe(5m / 6, 1m / 6);
}