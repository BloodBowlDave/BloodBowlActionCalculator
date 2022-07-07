using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class PassParser : IActionParser
	{
        public Action Parse(string input)
		{
			var usePro = input.Contains("*");
			var rerollInaccuratePass = !input.Contains("'");

			input = input.Replace("*", "").Replace("'", "");

			int roll;
			if (input.Length == 4)
			{
				roll = int.Parse(input.Substring(1, 1));
				var modifier = int.Parse(input.Substring(3, 1));
				modifier = input.Substring(2, 1) == "-" ? -modifier : modifier;
				
				return new Pass(usePro, rerollInaccuratePass, roll, modifier);
			}

			roll = int.Parse(input[1..]);

			return new Pass(usePro, rerollInaccuratePass, roll, 0);
		}
	}
}