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

			var useProBeforeReroll = input.Contains("*");
			var useBrawlerBeforeReroll = input.Contains("^");
			var rerollNonCriticalFailure = !input.Contains("'");
			var affectedByDivingTackle = input.Contains("\"");

			input = input.Replace("*", "").Replace("^", "")
		        .Replace("'", "").Replace("\"", "");

			var actionType = GetActionType(input);

			var action = actionType switch
			{
				ActionType.Block => BlockAction(input),
				ActionType.Pass => PassAction(input),
				_ => OtherAction(input, actionType)
			};

			action.UseProBeforeReroll = useProBeforeReroll;
			action.UseBrawlerBeforeReroll = useBrawlerBeforeReroll;
			action.RerollNonCriticalFailure = rerollNonCriticalFailure;
			action.AffectedByDivingTackle = affectedByDivingTackle;

			return action;
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
	        var numberOfDice = int.Parse(split[0]);
	        var numberOfSuccessfulResults = int.Parse(split[1]);

	        return BuildAction(numberOfDice, numberOfSuccessfulResults);
		}

        private static Action BuildAction(int numberOfDice, int numberOfSuccessfulResults)
		{
			var successOnOneDie = (decimal)numberOfSuccessfulResults / 6;
			decimal success;
			decimal failure;
			decimal successOnTwoDice;

			switch (numberOfDice)
	        {
		        case -3:
			        success = (decimal) Math.Pow((double) successOnOneDie, 3);
			        failure = 1 - success;
			        successOnTwoDice = (decimal) Math.Pow((double) successOnOneDie, 2);

			        return new Action(ActionType.Block, success, failure, 0)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults,
				        SuccessOnOneDie = successOnOneDie,
				        SuccessOnTwoDice = successOnTwoDice
			        };
		        case -2:
			        success = (decimal) Math.Pow((double) successOnOneDie, 2);
			        failure = 1 - success;

			        return new Action(ActionType.Block, success, failure, 0)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults,
				        SuccessOnOneDie = successOnOneDie
			        };
				case 1:
			        success = successOnOneDie;
			        failure = 1 - success;

			        return new Action(ActionType.Block, success, failure, 0)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults,
				        SuccessOnOneDie = successOnOneDie
			        };
				case 2:
			        failure = (decimal) Math.Pow(1 - (double) successOnOneDie, 2);
			        success = 1 - failure;

			        return new Action(ActionType.Block, success, failure, 0)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults,
				        SuccessOnOneDie = successOnOneDie
			        };
				case 3:
			        failure = (decimal) Math.Pow(1 - (double) successOnOneDie, 3);
			        success = 1 - failure;
			        successOnTwoDice = 1 - (decimal) Math.Pow((double) successOnOneDie, 2);

			        return new Action(ActionType.Block, success, failure, 0)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults,
				        SuccessOnOneDie = successOnOneDie,
				        SuccessOnTwoDice = successOnTwoDice
			        };
				default:
					throw new ArgumentOutOfRangeException(nameof(numberOfDice));
	        }
        }

        private static Action OtherAction(string input, ActionType actionType)
        {
	        if (input.Contains("/"))
	        {
		        var split = input.Split('/');
		        var numerator = split[0];
		        var denominator = split[1];
		        var success = decimal.Parse(numerator[1..]) / decimal.Parse(denominator);

                return new Action(actionType, success, 1 - success, 0);
			}
	        else
            {
                var roll = int.Parse(input.Length == 2 ? input[1..] : input);
                var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
				
                var action = new Action(actionType, success, 1 - success, 0)
                {
	                OriginalRoll = roll
                };

                return action;
			}
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
				
		        var successes = 7m - modifiedRoll.ThisOrMinimum(2).ThisOrMaximum(6);
		        var failures = 6m - successes + (modifierIsNegative ? modifier : 0);
		        failures = failures > 5 ? 5 : failures;
		        var nonCriticalFailures = 6m - successes - failures;
		        
		        return new Action(ActionType.Pass, 
			        successes / 6, 
			        failures / 6, 
			        nonCriticalFailures / 6);
	        }

	        var success = (7m - int.Parse(input[1..])) / 6;
            var failure = 1m / 6;
	        var nonCriticalFailure = 1 - success - failure;

	        return new Action(ActionType.Pass, success, failure, nonCriticalFailure);
        }
    }
}
