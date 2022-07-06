using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;


namespace ActionCalculator.ActionBuilders
{
	public class DauntlessParser : IActionParser
	{
		private readonly ID6 _d6;

		public DauntlessParser(ID6 d6)
		{
			_d6 = d6;
		}

		public Action Parse(string input)
		{
			var usePro = input.Contains("*");
			var rerollFailure = !input.Contains("'");

			input = input.Replace("*", "").Replace("'", "");

			var roll = int.Parse(input.Length == 2 ? input[1..] : input);
			var success = _d6.Success(1, roll);

			return new Dauntless(success, 1 - success, rerollFailure, roll, usePro);
		}

        public Action Build(int roll)
		{
			var success = _d6.Success(1, roll);

            return new Dauntless(success, 1 - success, true, roll, false);
		}

        public Action Build(int roll, int modifier)
        {
            throw new NotImplementedException();
        }
    }
}