using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class ArmourBreakBuilder : IActionBuilder
	{
		public Action Build(string input) => new ArmourBreak(int.Parse(input[1..]));
	}
}