using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ActionBuilders;

public class ArmourBreakActionBuilder : IActionBuilder
{
    public Action Build(string input) => new ArmourBreak(int.Parse(input[1..]));
}