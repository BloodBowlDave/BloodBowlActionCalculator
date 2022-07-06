using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class ChainsawParser : IActionParser
	{
		public Action Parse(string input) => Build(int.Parse(input[1..]));

        public Action Build(int roll) => new Chainsaw(roll);

        public Action Build(int roll, int modifier) => throw new NotImplementedException();
    }
}