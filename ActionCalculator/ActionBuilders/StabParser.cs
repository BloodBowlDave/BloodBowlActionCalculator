using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class StabParser : IActionParser
	{
		public Action Parse(string input) => new Stab(int.Parse(input[1..]));
	}
}