using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ActionBuilders;

public class FoulActionBuilder : IActionBuilder
{
    public Action Build(string input) => new Foul(int.Parse(input[1..]));
}