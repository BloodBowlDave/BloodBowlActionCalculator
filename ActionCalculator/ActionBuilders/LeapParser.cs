using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders
{
    public class LeapParser : IActionParser
    {
        public Action Parse(string input)
        {
            var usePro = input.Contains("*");
            var useDivingTackle = input.Contains("\"");

            input = input.Replace("*", "").Replace("\"", "");

            var roll = int.Parse(input[1..]);

            return new Leap(roll, usePro, useDivingTackle);
        }
    }
}
