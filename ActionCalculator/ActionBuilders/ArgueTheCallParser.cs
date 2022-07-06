using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class ArgueTheCallParser : IActionParser
	{
		private readonly ID6 _d6;

		public ArgueTheCallParser(ID6 d6)
		{
			_d6 = d6;
		}

		public Action Parse(string input)
		{
			var roll = int.Parse(input.Length == 2 ? input[1..] : input);
            return Build(roll);
        }

        public Action Build(int roll)
		{
			var success = _d6.Success(1, roll);
            const decimal criticalFailure = 1m / 6;

            return new ArgueTheCall(success, 1m - success - criticalFailure, criticalFailure, roll);
		}

        public Action Build(int roll, int modifier) => throw new NotImplementedException();
    }
}