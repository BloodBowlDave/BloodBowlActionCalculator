using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ActionBuilders;

public class PassActionBuilder : IActionBuilder
{
    public Action Build(string input)
    {
        var usePro = input.Contains("*");
        var rerollInaccuratePass = !input.Contains("'");

        input = input.Replace("*", "").Replace("'", "");

        int roll;
        if (input.Length == 4)
        {
            roll = int.Parse(input.Substring(1, 1));
            var modifier = int.Parse(input.Substring(3, 1));
            modifier = input.Substring(2, 1) == "-" ? -modifier : modifier;

            var modifiedRoll = roll - modifier;

            var successes = (7m - modifiedRoll).ThisOrMinimum(1).ThisOrMaximum(5);
            var failures = (1m - modifier).ThisOrMinimum(1).ThisOrMaximum(5);
            var inaccuratePasses = 6m - successes - failures;

            return new Pass(successes / 6,
                failures / 6,
                inaccuratePasses / 6,
                usePro, 
                rerollInaccuratePass,
                roll,
                modifier);
        }

        roll = int.Parse(input[1..]);
        var success = (7m - roll) / 6;
        var failure = 1m / 6;
        var inaccuratePass = 1 - success - failure;

        return new Pass(success,
            failure,
            inaccuratePass,
            usePro,
            rerollInaccuratePass,
            roll,
            0);
    }
}