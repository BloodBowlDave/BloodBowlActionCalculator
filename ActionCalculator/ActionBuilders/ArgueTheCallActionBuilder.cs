using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class ArgueTheCallActionBuilder : IActionBuilder
{
    public Action Build(string input)
    {
        var roll = int.Parse(input.Length == 2 ? input[1..] : input);
        var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
        const decimal criticalFailure = 1m / 6;

        return new ArgueTheCall(success, 1m - success - criticalFailure, criticalFailure, roll);
    }
}