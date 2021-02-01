using System;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
    public class ActionParser : IActionParser
    {
        public Action Parse(string input)
        {
            input = input.ToUpperInvariant();

            if (input.Contains('D'))
            {
                var split = input.Split('D');
                var blockDice = int.Parse(split[0]);
                var successPerDice = (double)int.Parse(split[1]) / 6;

                if (blockDice < 2)
                {
                    var success = (decimal)Math.Pow(successPerDice, Math.Abs(blockDice));
                    var failure = 1 - success;

                    return new Action(success, failure, input);
                }
                else
                {
                    var failure = (decimal)Math.Pow(1 - successPerDice, blockDice);
                    var success = 1 - failure;

                    return new Action(success, failure, input);
                }
            }
            else
            {
                var success = (7m - int.Parse(input)) / 6;
                var failure = 1 - success;

                return new Action(success, failure, input);
            }
        }
    }
}
