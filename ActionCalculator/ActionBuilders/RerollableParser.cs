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
    
			var roll = int.Parse(input.Length == 2 ? input[1..] : input);

			return new Rerollable(roll, usePro);
		}
	}
}