using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class RerollableParser : IActionParser
	{
        public Action Parse(string input)
		{
			var usePro = input.Contains("*");

            input = input.Replace("*", "");
			
			var roll = int.Parse(input);

			return new Rerollable(roll, usePro);
		}
	}
}