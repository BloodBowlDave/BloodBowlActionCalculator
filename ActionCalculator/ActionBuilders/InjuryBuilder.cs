using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class InjuryBuilder : IActionBuilder
	{
		public Action Build(string input) => new Injury(int.Parse(input[1..]));
	}
}