using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class BlockBuilder : IActionBuilder
{
    public Action Build(string input)
    {
        var usePro = input.Contains("*");
        var useBrawler = input.Contains("^");
        var rerollNonCriticalFailure = !input.Contains("'");

        input = input.Replace("*", "").Replace("^", "").Replace("'", "");

        var split = input.Split('D');
        var numberOfDice = int.Parse(split[0]);
        var resultsSplit = split[1].Split('!');
        var numberOfSuccessfulResults = int.Parse(resultsSplit[0]);
        var numberOfNonCriticalFailures = resultsSplit.Length > 1 ? int.Parse(resultsSplit[1]) : 0;

        return new Block(numberOfDice, numberOfSuccessfulResults, numberOfNonCriticalFailures, useBrawler, usePro, rerollNonCriticalFailure);
    }
}