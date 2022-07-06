using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
	public class DodgeParser : IActionParser
	{
		private readonly ID6 _d6;

		public DodgeParser(ID6 d6)
		{
			_d6 = d6;
		}

		public Action Parse(string input)
		{
			var usePro = input.Contains("*");
			var useDivingTackle = input.Contains("\"");
			var useBreakTackle = !input.Contains("¬");

			input = input.Replace("*", "").Replace("¬", "").Replace("\"", "");

			var roll = int.Parse(input.Length == 2 ? input[1..] : input);
			var success = _d6.Success(1, roll);

			return new Dodge(success, 1 - success, roll, usePro, useDivingTackle, useBreakTackle);
		}

        public Action Build(int roll)
		{
			var success = _d6.Success(1, roll);

            return new Dodge(success, 1 - success, roll, false, false, true);
		}

        public Action Build(int roll, int modifier) => throw new NotImplementedException();
    }
}