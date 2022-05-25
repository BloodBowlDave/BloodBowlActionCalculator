using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class NonRerollableActionBuilder : IActionBuilder
{
    public Action Build(string input)
    {
        if (input.Contains("/"))
        {
            var split = input.Split('/');
            var numerator = int.Parse(split[0][1..]);
            var denominator = int.Parse(split[1]);
            var success = (decimal)numerator / denominator;

            return new NonRerollableAction(success, 1 - success, numerator, denominator);
        }
        else
        {
            var roll = int.Parse(input.Length == 2 ? input[1..] : input);
            var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
            
            return new NonRerollableAction(success, 1 - success, roll, 6);
        }
    }
}