using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class LandingBuilder : IActionBuilder
	{
		private readonly ID6 _d6;

		public LandingBuilder(ID6 d6)
		{
			_d6 = d6;
		}

		public Action Build(string input)
		{
			var usePro = input.Contains("*");

			input = input.Replace("*", "");

			var roll = int.Parse(input.Length == 2 ? input[1..] : input);
			var success = _d6.Success(1, roll);

			return new Landing(success, 1 - success, roll, usePro);
		}
	}
}