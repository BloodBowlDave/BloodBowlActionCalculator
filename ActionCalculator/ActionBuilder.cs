using System;
using System.Linq;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
    public class ActionBuilder : IActionBuilder
    {
        public Action Build(string input, Skills skills)
        {
            input = input.ToUpperInvariant();
            
            if (input.Skip(1).Contains('D'))
            {
                var split = input.Split('D');
                var blockDice = int.Parse(split[0]);
                var successPerDice = (double)int.Parse(split[1]) / 6;

                if (blockDice < 2)
                {
                    var success = (decimal)Math.Pow(successPerDice, Math.Abs(blockDice));
                    var failure = 1 - success;

                    return new Action(ActionType.Block, success, failure);
                }
                else
                {
                    var failure = (decimal)Math.Pow(1 - successPerDice, blockDice);
                    var success = 1 - failure;

                    return new Action(ActionType.Block, success, failure);
                }
            }

            var actionTypeIsDefined = Enum.IsDefined(typeof(ActionType), (int)input[0]);

            if (actionTypeIsDefined)
            {
                var failure = 1 - (7m - int.Parse(input[1..])) / 6;
                var actionType = (ActionType) input[0];

                if (HasSkillReroll(actionType, skills))
                {
                    failure *= failure;
                }

                return new Action(actionType, 1 - failure, failure);
            }
            else
            {
                var success = (7m - int.Parse(input)) / 6;
                var failure = 1 - success;

                return new Action(ActionType.Other, success, failure);
            }
        }

        private static bool HasSkillReroll(ActionType actionType, Skills skills) =>
            actionType switch
            {
                ActionType.PickUp when skills.HasFlag(Skills.SureHands) => true,
                ActionType.Pass when skills.HasFlag(Skills.Pass) => true,
                ActionType.Catch when skills.HasFlag(Skills.Catch) => true,
                _ => false
            };
    }
}
