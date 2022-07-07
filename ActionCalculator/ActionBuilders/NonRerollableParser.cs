using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class NonRerollableParser : IActionParser
	{
		public Action Parse(string input)
		{
			if (input.Contains("/"))
			{
				var split = input.Split('/');
				var numerator = int.Parse(split[0][1..]);
				var denominator = int.Parse(split[1]);

				return new NonRerollable(numerator, denominator);
			}

			var roll = int.Parse(input.Length == 2 ? input[1..] : input);

			return new NonRerollable(roll, 6);
		}
	}
}