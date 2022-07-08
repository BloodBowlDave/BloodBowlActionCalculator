using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class ThrowTeammateParser : IActionParser
	{
        public Action Parse(string input)
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
				
				return new ThrowTeammate(roll, modifier, usePro, rerollInaccurateThrow);
			}

			roll = int.Parse(input[1..]);

			return new ThrowTeammate(roll, 0, usePro, rerollInaccurateThrow);
		}
	}
}