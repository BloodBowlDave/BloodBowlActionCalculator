using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators.Block;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ProbabilityCalculators.Block
{
	public class BrawlerCalculator : IBrawlerCalculator
	{
		public decimal FailButRollBothDown(Action action) =>
			action.NumberOfDice switch
			{
				-3 => 0,
				-2 => 0,
				1 => 1m / 6,
				2 => (11m - 2 * action.NumberOfSuccessfulResults) / 36,
				3 => action.NumberOfSuccessfulResults switch
				{
					1 => 61m / 216,
					2 => 37m / 216,
					3 => 19m / 216,
					4 => 7m / 216,
					_ => throw new ArgumentOutOfRangeException(nameof(action.NumberOfSuccessfulResults))
				},
				_ => throw new ArgumentOutOfRangeException(nameof(action.NumberOfDice))
			};

		public bool UseBrawler(int r, PlayerAction playerAction) =>
			(r == 0 || playerAction.Action.UseBrawlerBeforeReroll) && playerAction.Player.HasSkill(Skills.Brawler);
	}
}
