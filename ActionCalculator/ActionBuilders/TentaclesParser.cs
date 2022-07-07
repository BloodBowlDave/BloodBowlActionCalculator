using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class TentaclesParser : IActionParser
	{
		public Action Parse(string input)
		{
			var usePro = input.Contains("*");
			var rerollFailure = !input.Contains("'");

			input = input.Replace("*", "").Replace("'", "");

			var difference = int.Parse(input[1..]);

			return new Tentacles(usePro, rerollFailure, difference);
		}
	}
}