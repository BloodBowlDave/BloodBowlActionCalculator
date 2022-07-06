using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class BribeParser : IActionParser
	{
		public Action Parse(string input) => new Bribe(5m / 6, 1m / 6);

        public Action Build(int roll) => throw new NotImplementedException();

        public Action Build(int roll, int modifier) => throw new NotImplementedException();
    }
}