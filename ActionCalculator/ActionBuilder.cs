using System;
using System.Linq;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
    public class ActionBuilder : IActionBuilder
    {
        private readonly ITwoD6 _twoD6;

        public ActionBuilder(ITwoD6 twoD6)
        {
            _twoD6 = twoD6;
        }

        public Action Build(string input)
        {
	        input = input.ToUpperInvariant();

			var usePro = input.Contains("*");
			var useBrawler = input.Contains("^");
			var rerollNonCriticalFailure = !input.Contains("'");
			var affectedByDivingTackle = input.Contains("\"");

			input = input.Replace("*", "").Replace("^", "")
		        .Replace("'", "").Replace("\"", "");

			var actionType = GetActionType(input);

			var action = actionType switch
			{
				ActionType.Block => BlockAction(input),
				ActionType.Foul => FoulAction(input),
				ActionType.ArmourBreak => ArmourBreakAction(input),
				ActionType.Pass => PassAction(input),
				ActionType.Bribe => BribeAction(),
				ActionType.ArgueTheCall => ArgueTheCallAction(input),
				ActionType.Tentacles => TentaclesAction(input),
				ActionType.Injury => InjuryAction(input),
				_ => OtherAction(input, actionType)
			};

			action.UsePro = usePro;
			action.UseBrawler = useBrawler;
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

	        return BuildBlockAction(numberOfDice, numberOfSuccessfulResults);
		}

        private Action FoulAction(string input)
        {
			var roll = int.Parse(input[1..]);
            var success = _twoD6.Success(roll.ThisOrMinimum(2).ThisOrMaximum(12));
			
			var action = new Action(ActionType.Foul, success, 1 - success, 0, success)
            {
                OriginalRoll = roll
			};

            return action;
		}
		
        private static Action BuildBlockAction(int numberOfDice, int numberOfSuccessfulResults)
		{
			var successOnOneDie = (decimal)numberOfSuccessfulResults / 6;
			decimal success, failure, successOnTwoDice;

			switch (numberOfDice)
	        {
		        case -3:
			        success = (decimal) Math.Pow((double) successOnOneDie, 3);
			        failure = 1 - success;
			        successOnTwoDice = (decimal) Math.Pow((double) successOnOneDie, 2);

			        return new Action(ActionType.Block, success, failure, 0, successOnOneDie)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults,
				        SuccessOnTwoDice = successOnTwoDice
			        };
		        case -2:
			        success = (decimal) Math.Pow((double) successOnOneDie, 2);
			        failure = 1 - success;

			        return new Action(ActionType.Block, success, failure, 0, successOnOneDie)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults
			        };
				case 1:
			        success = successOnOneDie;
			        failure = 1 - success;

			        return new Action(ActionType.Block, success, failure, 0, successOnOneDie)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults
			        };
				case 2:
			        failure = (decimal) Math.Pow(1 - (double) successOnOneDie, 2);
			        success = 1 - failure;

			        return new Action(ActionType.Block, success, failure, 0, successOnOneDie)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults
			        };
				case 3:
			        failure = (decimal) Math.Pow(1 - (double) successOnOneDie, 3);
			        success = 1 - failure;
			        successOnTwoDice = 1 - (decimal) Math.Pow(1 - (double) successOnOneDie, 2);

			        return new Action(ActionType.Block, success, failure, 0, successOnOneDie)
			        {
				        NumberOfDice = numberOfDice,
				        NumberOfSuccessfulResults = numberOfSuccessfulResults,
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

                return new Action(actionType, success, 1 - success, 0, success);
			}
	        else
            {
                var roll = int.Parse(input.Length == 2 ? input[1..] : input);
                var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
				
                var action = new Action(actionType, success, 1 - success, 0, success)
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
			        nonCriticalFailures / 6,
                    successes / 6);
	        }

	        var success = (7m - int.Parse(input[1..])) / 6;
            var failure = 1m / 6;
	        var nonCriticalFailure = 1 - success - failure;

	        return new Action(ActionType.Pass, success, failure, nonCriticalFailure, success);
		}

		private static Action BribeAction() => 
            new(ActionType.Bribe, 5m / 6, 1m / 6, 0, 0);


        private static Action ArgueTheCallAction(string input)
        {
            var roll = int.Parse(input.Length == 2 ? input[1..] : input);
            var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;

            return new Action(ActionType.ArgueTheCall, success, 1m / 6, 1 - success - 1m / 6, 0);
		}

        private Action ArmourBreakAction(string input)
		{
            var roll = int.Parse(input[1..]);
            var success = _twoD6.Success(roll.ThisOrMinimum(2).ThisOrMaximum(12));

            var action = new Action(ActionType.ArmourBreak, success, 1 - success, 0, success)
            {
                OriginalRoll = roll
            };

            return action;
		}

        private Action InjuryAction(string input)
		{
			var roll = int.Parse(input[1..]);
            var success = _twoD6.Success(roll.ThisOrMinimum(2).ThisOrMaximum(12));

            var action = new Action(ActionType.Injury, success, 1 - success, 0, success)
            {
                OriginalRoll = roll
            };

            return action;
		}

        private Action TentaclesAction(string input)
        {
            decimal failure;

            if (input.Length == 4)
            {
                var tentaclesStrength = int.Parse(input.Substring(1, 1));
                var playerStrength = int.Parse(input.Substring(3, 1));

				failure = (decimal)(tentaclesStrength - playerStrength + 1).ThisOrMinimum(1).ThisOrMaximum(6) / 6;
				
                return new Action(ActionType.Tentacles, 1 - failure, failure, 0, 1 - failure);

            }

            var difference = int.Parse(input[1..]);

            failure = (decimal)(difference + 1).ThisOrMinimum(1).ThisOrMaximum(6) / 6;

            return new Action(ActionType.Tentacles, 1 - failure, failure, 0, 1 - failure);
		}
	}
}
