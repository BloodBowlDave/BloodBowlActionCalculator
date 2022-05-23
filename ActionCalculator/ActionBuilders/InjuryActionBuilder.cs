using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ActionBuilders;

public class InjuryActionBuilder : IActionBuilder
{
    public Action Build(string input) => new Injury(int.Parse(input[1..]));
}