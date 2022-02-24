using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators.Block;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ProbabilityCalculators.Block
{
	public class BrawlerCalculator : IBrawlerCalculator
	{
		public decimal ProbabilityCanUseBrawler(Action action) =>
			action.NumberOfDice switch
			{
				-3 => action.NumberOfSuccessfulResults switch {
					1 => 3m/ 216,
					2 => 12m / 216,
					3 => 27m / 216,
					4 => 48m / 216,
					_ => 0
				},
				-2 => action.NumberOfSuccessfulResults * (1m / 18),
				1 => 1m / 6,
				2 => (11m - 2 * action.NumberOfSuccessfulResults) / 36,
				3 => action.NumberOfSuccessfulResults switch
				{
					1 => 61m / 216,
					2 => 37m / 216,
					3 => 19m / 216,
					4 => 7m / 216,
					_ => 0
				},
				_ => throw new ArgumentOutOfRangeException(nameof(action.NumberOfDice))
			};

		public bool UseBrawler(int r, PlayerAction playerAction) =>
			(r == 0 || playerAction.Action.UseBrawlerBeforeReroll) 
			&& playerAction.Player.HasSkill(Skills.Brawler);

		public decimal ProbabilityCanUseBrawlerAndPro(Action action) =>
			action.NumberOfDice switch
			{
				-3 => action.NumberOfSuccessfulResults switch
				{
					1 => 19m / 216,
					2 => 34m / 216,
					4 => 28m / 216,
					_ => 0
				},
				-2 => action.NumberOfSuccessfulResults switch
				{
					1 => 9m / 36,
					2 => 7m / 36,
					4 => 3m / 36,
					_ => 0
				},
				_ => throw new ArgumentOutOfRangeException()
			};
	}
}
