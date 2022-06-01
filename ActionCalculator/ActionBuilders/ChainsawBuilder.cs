using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class ChainsawBuilder : IActionBuilder
{
    public Action Build(string input) => new Chainsaw(int.Parse(input[1..]));
}