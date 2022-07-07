using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class LandingParser : IActionParser
	{
        public Action Parse(string input)
		{
			var usePro = input.Contains("*");

			input = input.Replace("*", "");

			var roll = int.Parse(input.Length == 2 ? input[1..] : input);

			return new Landing(roll, usePro);
		}
	}
}