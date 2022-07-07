using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class FoulParser : IActionParser
	{
		public Action Parse(string input) => new Foul(int.Parse(input[1..]));
    }
}