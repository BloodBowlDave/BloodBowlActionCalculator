using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class ShadowingBuilder : IActionBuilder
	{
		public Action Build(string input)
		{
			var usePro = input.Contains("*");
			var rerollFailure = !input.Contains("'");

			input = input.Replace("*", "").Replace("'", "");

			var difference = int.Parse(input[1..]);
			var failure = (decimal)(difference + 1).ThisOrMinimum(1).ThisOrMaximum(6) / 6;

			return new Shadowing(1 - failure, failure, usePro, rerollFailure, difference);
		}
	}
}