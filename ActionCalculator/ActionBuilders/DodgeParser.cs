using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class DodgeParser : IActionParser
	{
        public Action Parse(string input)
		{
			var usePro = input.Contains("*");
			var useDivingTackle = input.Contains("\"");
			var useBreakTackle = !input.Contains("¬");

			input = input.Replace("*", "").Replace("¬", "").Replace("\"", "");

			var roll = int.Parse(input.Length == 2 ? input[1..] : input);

			return new Dodge(roll, usePro, useDivingTackle, useBreakTackle);
		}
    }
}