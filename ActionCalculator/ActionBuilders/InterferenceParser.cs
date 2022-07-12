using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class InterferenceParser : IActionParser
	{
        public Action Parse(string input)
		{
			var roll = int.Parse(input.Length == 2 ? input[1..] : input);

			return new Interference(roll);
		}
	}
}