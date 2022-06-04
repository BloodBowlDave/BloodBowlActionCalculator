using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class ArgueTheCallBuilder : IActionBuilder
{
    private readonly ID6 _d6;

    public ArgueTheCallBuilder(ID6 d6)
    {
        _d6 = d6;
    }

    public Action Build(string input)
    {
        var roll = int.Parse(input.Length == 2 ? input[1..] : input);
        var success = _d6.Success(1, roll);
        const decimal criticalFailure = 1m / 6;

        return new ArgueTheCall(success, 1m - success - criticalFailure, criticalFailure, roll);
    }
}