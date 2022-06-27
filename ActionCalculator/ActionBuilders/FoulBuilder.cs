using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class FoulBuilder : IActionBuilder
	{
		public Action Build(string input) => new Foul(int.Parse(input[1..]));
	}
}