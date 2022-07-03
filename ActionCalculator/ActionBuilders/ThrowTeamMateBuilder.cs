using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class ThrowTeammateBuilder : IActionBuilder
	{
		private readonly ID6 _d6;

		public ThrowTeammateBuilder(ID6 d6)
		{
			_d6 = d6;
		}

		public Action Build(string input)
		{
			var usePro = input.Contains("*");
			var rerollInaccurateThrow = !input.Contains("'");

			input = input.Replace("*", "").Replace("'", "");

			int roll;
			if (input.Length == 4)
			{
				roll = int.Parse(input.Substring(1, 1));
				var modifier = int.Parse(input.Substring(3, 1));
				modifier = input.Substring(2, 1) == "-" ? -modifier : modifier;

				var modifiedRoll = roll - modifier;

				var successes = (7m - modifiedRoll).ThisOrMinimum(1).ThisOrMaximum(5);
				var failures = (1m - modifier).ThisOrMinimum(1).ThisOrMaximum(5);
				var inaccurateThrows = 6 - successes - failures;

				return new ThrowTeammate(successes / 6,
					failures / 6,
					inaccurateThrows / 6,
					usePro,
					rerollInaccurateThrow,
					roll,
					modifier);
			}

			roll = int.Parse(input[1..]);
			var success = _d6.Success(1, roll);
			var failure = 1m / 6;
			var inaccurateThrow = 1 - success - failure;

			return new ThrowTeammate(success,
				failure,
				inaccurateThrow,
				usePro,
				rerollInaccurateThrow,
				roll,
				0);
		}
	}
}