using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.ActionBuilders
{
	public class ArgueTheCallBuilder : IActionBuilder
	{
		private readonly ID6 _d6;

		public ArgueTheCallBuilder(ID6 d6)
		{
			_d6 = d6;
		}

		public Models.Actions.Action Build(string input)
		{
			var roll = int.Parse(input.Length == 2 ? input[1..] : input);
			var success = _d6.Success(1, roll);
			const decimal criticalFailure = 1m / 6;

			return new ArgueTheCall(success, 1m - success - criticalFailure, criticalFailure, roll);
		}
	}
}