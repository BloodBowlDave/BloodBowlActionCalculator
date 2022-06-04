using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class InterceptionBuilder : IActionBuilder
{
    private readonly ID6 _d6;

    public InterceptionBuilder(ID6 d6)
    {
        _d6 = d6;
    }

    public Action Build(string input)
    {
        var roll = int.Parse(input.Length == 2 ? input[1..] : input);
        var success = _d6.Success(1, roll);

        return new Interception(1 - success, roll);
    }
}