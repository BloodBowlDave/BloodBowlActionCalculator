using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class BribeActionBuilder : IActionBuilder
{
    public Action Build(string input) => new Bribe(5m / 6, 1m / 6);
}