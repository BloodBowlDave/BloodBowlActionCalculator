using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators;
using ActionCalculator.Abstractions.ProbabilityCalculators.Block;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.ProbabilityCalculators
{
	public class TwoDiceBlockCalculator : IProbabilityCalculator
	{
		private readonly IProbabilityCalculator _probabilityCalculator;
		private readonly IProCalculator _proCalculator;
		private readonly IBrawlerCalculator _brawlerCalculator;

		public TwoDiceBlockCalculator(IProbabilityCalculator probabilityCalculator, 
			IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
		{
			_probabilityCalculator = probabilityCalculator;
			_proCalculator = proCalculator;
			_brawlerCalculator = brawlerCalculator;
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

			if (_brawlerCalculator.UseBrawler(r, playerAction))
			{
				usedBrawler = _brawlerCalculator.FailButRollBothDown(action);
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
	}
}
