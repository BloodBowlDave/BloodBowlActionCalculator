using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
	public class ActionCalculator : IActionCalculator
	{
		private readonly ICalculationBuilder _calculationBuilder;
		private readonly IProbabilityCalculator _probabilityCalculator;

		private const decimal OneInSix = 1m / 6;
		private const decimal ScatterToTargetSquare = 24m / 512;
		private const decimal ScatterToTargetOrAdjacentSquare = 240m / 512;
		
		public ActionCalculator(
			ICalculationBuilder calculationBuilder, 
			IProbabilityCalculator probabilityCalculator)
		{
			_calculationBuilder = calculationBuilder;
			_probabilityCalculator = probabilityCalculator;
		}

		public CalculationResult Calculate(string calculationString)
		{
			var calculation = _calculationBuilder.Build(calculationString);
			
			var probabilities = _probabilityCalculator.Calculate(calculation).ToList();

			RemoveBlanks(probabilities);
			AggregateResults(probabilities);

			var passIndex = GetPassIndex(calculation);

			if (passIndex == -1) 
			{ 
				return new CalculationResult(probabilities);
			}

			var inaccuratePassCalculation = GetInaccuratePassCalculation(calculation, passIndex);

			var inaccuratePassProbabilities = _probabilityCalculator.Calculate(inaccuratePassCalculation).ToList();

			for (var i = 0; i < inaccuratePassProbabilities.Count; i++)
			{
				probabilities[i] += inaccuratePassProbabilities[i];
			}

			return new CalculationResult(probabilities);
		}

		private static Calculation GetInaccuratePassCalculation(
			Calculation calculation, int passActionIndex)
		{
			var playerActions = new List<PlayerAction>();

			for (var i = 0; i < calculation.PlayerActions.Length; i++)
			{
				if (i == passActionIndex)
				{
					var pass = calculation.PlayerActions[i];
					var inaccuratePassAction = new Action(
						ActionType.InaccuratePass,
						pass.Action.Success,
						pass.Action.Failure,
						pass.Action.NonCriticalFailure);

					playerActions.Add(new PlayerAction(pass.Player, inaccuratePassAction));
				}
				else if (i == passActionIndex + 1)
				{
					var playerAction = calculation.PlayerActions[i];
					if (playerAction.Action.ActionType != ActionType.Catch)
					{
						continue;
					}

					if (playerAction.Player.Skills.HasFlag(Skills.DivingCatch))
					{
						playerActions.Add(new PlayerAction(playerAction.Player,
							new Action(ActionType.OtherNonRerollable,
								ScatterToTargetOrAdjacentSquare,
								1m - ScatterToTargetOrAdjacentSquare)));
					}
					else
					{
						playerActions.Add(new PlayerAction(playerAction.Player,
							new Action(ActionType.OtherNonRerollable,
								ScatterToTargetSquare,
								1m - ScatterToTargetSquare)));
					}

					playerActions.Add(GetInaccurateCatch(playerAction));
				}
				else
				{
					playerActions.Add(calculation.PlayerActions[i]);
				}
			}

			return new Calculation(playerActions.ToArray());
		}

		private static PlayerAction GetInaccurateCatch(PlayerAction playerAction) =>
			playerAction.Action.Success > OneInSix
				? new PlayerAction(playerAction.Player,
					new Action(ActionType.Catch,
						playerAction.Action.Success - OneInSix,
						playerAction.Action.Failure + OneInSix))
				: playerAction;

		private static int GetPassIndex(Calculation calculation)
		{
			for (var i = 0; i < calculation.PlayerActions.Length; i++)
			{
				var playerAction = calculation.PlayerActions[i];
				if (playerAction.Action.ActionType == ActionType.Pass 
				    && playerAction.Action.NonCriticalFailure > 0)
				{
					return i;
				}
			}

			return -1;
		}
		
		private static void RemoveBlanks(IList<decimal> result)
		{
			for (var i = result.Count - 1; i >= 0; i--)
			{
				if (result[i] == 0)
				{
					result.RemoveAt(i);
				}
				else
				{
					break;
				}
			}
		}

		private static void AggregateResults(IList<decimal> result)
		{
			for (var i = 1; i < result.Count; i++)
			{
				result[i] += result[i - 1];
			}
		}
	}
}