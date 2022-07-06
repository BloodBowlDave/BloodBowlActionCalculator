using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class InjuryParser : IActionParser
	{
		public Action Parse(string input) => new Injury(int.Parse(input[1..]));
	}
}