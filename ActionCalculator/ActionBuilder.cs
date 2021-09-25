using System;
using System.Linq;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
    public class ActionBuilder : IActionBuilder
    {
        public Action Build(string input)
        {
	        input = input.ToUpperInvariant();

	        var actionType = GetActionType(input);

	        return actionType switch
	        {
		        ActionType.Block => BlockAction(input),
		        ActionType.Pass => PassAction(input),
		        ActionType.InaccuratePass => InaccuratePassAction(input),
		        _ => OtherAction(input, actionType)
	        };
        }

        private static ActionType GetActionType(string input) =>
	        IsBlockAction(input) 
		        ? ActionType.Block 
		        : Enum.IsDefined(typeof(ActionType), (int) input[0]) 
			        ? (ActionType) input[0] 
			        : ActionType.Other;

        private static bool IsBlockAction(string input) => input.Skip(1).Contains('D');

        private static Action BlockAction(string input)
        {
	        var split = input.Split('D');
	        var blockDice = int.Parse(split[0]);
	        var successPerDice = (double) int.Parse(split[1]) / 6;

	        if (blockDice < 2)
	        {
		        var success = (decimal) Math.Pow(successPerDice, Math.Abs(blockDice));
		        var failure = 1 - success;

		        return new Action(ActionType.Block, success, failure);
	        }
	        else
	        {
		        var failure = (decimal) Math.Pow(1 - successPerDice, blockDice);
		        var success = 1 - failure;

		        return new Action(ActionType.Block, success, failure);
	        }
        }

        private static Action OtherAction(string input, ActionType actionType)
        {
	        decimal success;

	        if (input.Contains("/"))
	        {
		        var split = input.Split('/');
		        var numerator = split[0];
		        var denominator = split[1];
		        success = decimal.Parse(numerator[1..]) / decimal.Parse(denominator);
			}
	        else
			{
				success = (7m - int.Parse(input.Length == 2 ? input[1..] : input)) / 6;
			}

	        return new Action(actionType, success, 1 - success);
        }

        private static Action PassAction(string input)
        {
	        if (input.Length == 4)
	        {
		        var roll = int.Parse(input.Substring(1, 1));
		        var modifier = int.Parse(input.Substring(3, 1));

		        var modifierIsNegative = input.Substring(2, 1) == "-";
		        modifier = modifierIsNegative ? -modifier : modifier;
            
		        var modifiedRoll = roll - modifier;
		        modifiedRoll = modifiedRoll > 6
			        ? 6 : modifiedRoll < 2
				        ? 2 : modifiedRoll;

		        var successes = 7m - modifiedRoll;
		        var failures = 6m - successes + (modifierIsNegative ? modifier : 0);
		        failures = failures > 5 ? 5 : failures;
		        var nonCriticalFailures = 6m - successes - failures;
		        
		        return new Action(ActionType.Pass, 
			        successes / 6, 
			        failures / 6, 
			        nonCriticalFailures / 6);
	        }

	        var success = (7m - int.Parse(input[1..])) / 6;
	        var failure = 1 - success;

	        return new Action(ActionType.Pass, success, failure);
        }

        private static Action InaccuratePassAction(string input)
        {
	        var passAction = PassAction(input);
	        return new Action(ActionType.InaccuratePass, passAction.Success, passAction.Failure,
		        passAction.NonCriticalFailure);
        }
	}
}
