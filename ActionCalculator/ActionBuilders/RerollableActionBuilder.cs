using System.Runtime.CompilerServices;
using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class RerollableActionBuilder : IActionBuilder
{
    private readonly ID6 _d6;

    public RerollableActionBuilder(ID6 d6)
    {
        _d6 = d6;
    }

    public Action Build(string input)
    {
        var usePro = input.Contains("*");
    
        var roll = int.Parse(input.Length == 2 ? input[1..] : input);
        var success = _d6.Success(1, roll);

        return new RerollableAction(success, 1 - success, usePro, roll);
    }
}