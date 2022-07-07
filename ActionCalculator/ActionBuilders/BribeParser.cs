using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class BribeParser : IActionParser
	{
		public Action Parse(string input) => new Bribe();
    }
}