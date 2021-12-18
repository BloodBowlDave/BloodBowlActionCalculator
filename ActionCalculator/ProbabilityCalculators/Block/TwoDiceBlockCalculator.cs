using System;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ProbabilityCalculators
{
	public class TwoDiceBlockCalculator : IProbabilityCalculator
	{
		private readonly IProbabilityCalculator _probabilityCalculator;
		private readonly IProCalculator _proCalculator;

		public TwoDiceBlockCalculator(IProbabilityCalculator probabilityCalculator, IProCalculator proCalculator)
		{
			_probabilityCalculator = probabilityCalculator;
			_proCalculator = proCalculator;
		}

		public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool inaccuratePass = false)
		{
			var player = playerAction.Player;
			var action = playerAction.Action;
			var success = playerAction.Action.Success;
			var failure = playerAction.Action.Failure;
			var oneDieSuccess = action.SuccessOnOneDie;

			_probabilityCalculator.Calculate(p * success, r, playerAction, usedSkills);

			var usedBrawler = 0m;

			if (UseBrawler(r, playerAction))
			{
				usedBrawler = RolledBothDown(action);
				_probabilityCalculator.Calculate(p * usedBrawler * oneDieSuccess, r, playerAction, usedSkills);

				var pBrawlerFails = p * usedBrawler * (1 - oneDieSuccess) * oneDieSuccess;

				if (_proCalculator.UsePro(playerAction, r, usedSkills))
				{
					_probabilityCalculator.Calculate(pBrawlerFails * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro);
					return;
				}

				if (r > 0)
				{
					_probabilityCalculator.Calculate(pBrawlerFails * player.LonerSuccess, r - 1, playerAction, usedSkills);
					return;
				}
			}

			p *= failure - usedBrawler;

			if (_proCalculator.UsePro(playerAction, r, usedSkills))
			{
				_probabilityCalculator.Calculate(p * player.ProSuccess * oneDieSuccess, r, playerAction, usedSkills | Skills.Pro);

				if (r > 0)
				{
					_probabilityCalculator.Calculate(p * (1 - player.ProSuccess * oneDieSuccess) * player.LonerSuccess * oneDieSuccess, r - 1, playerAction, usedSkills | Skills.Pro);
					return;
				}
			}

			if (r > 0)
			{
				_probabilityCalculator.Calculate(p * player.LonerSuccess * success, r - 1, playerAction, usedSkills);
			}
        }
		
		private static decimal RolledBothDown(Action action) =>
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

		private static bool UseBrawler(int r, PlayerAction playerAction) => 
			(r == 0 || playerAction.Action.UseBrawlerBeforeReroll) && playerAction.Player.HasSkill(Skills.Brawler);
	}
}
