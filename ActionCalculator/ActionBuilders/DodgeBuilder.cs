using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class DodgeBuilder : IActionBuilder
{
    public Action Build(string input)
    {
        var usePro = input.Contains("*");
        var useDivingTackle = input.Contains("\"");
        var useBreakTackle = !input.Contains("¬");

        input = input.Replace("*", "").Replace("¬", "").Replace("\"", "");

        var roll = int.Parse(input.Length == 2 ? input[1..] : input);
        var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;

        return new Dodge(success, 1 - success, roll, usePro, useDivingTackle, useBreakTackle);
    }
}