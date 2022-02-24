using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators;
using ActionCalculator.Abstractions.ProbabilityCalculators.Block;

namespace ActionCalculator.ProbabilityCalculators.Block
{
	public class ThreeDiceBlockCalculator : IProbabilityCalculator
	{
		private readonly IProbabilityCalculator _probabilityCalculator;
		private readonly IProCalculator _proCalculator;
		private readonly IBrawlerCalculator _brawlerCalculator;

		public ThreeDiceBlockCalculator(IProbabilityCalculator probabilityCalculator,
			IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
		{
			_probabilityCalculator = probabilityCalculator;
			_proCalculator = proCalculator;
			_brawlerCalculator = brawlerCalculator;
		}

		public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
		{
			var player = playerAction.Player;
			var action = playerAction.Action;
			var success = action.Success;
			var oneDieSuccess = action.SuccessOnOneDie;

			_probabilityCalculator.Calculate(p * success, r, playerAction, usedSkills);

			var failButRollBothDown = 0m;

			if (_brawlerCalculator.UseBrawler(r, playerAction))
			{
				failButRollBothDown = _brawlerCalculator.ProbabilityCanUseBrawler(action);
				_probabilityCalculator.Calculate(p * failButRollBothDown * oneDieSuccess, r, playerAction, usedSkills);

				var pBrawler = p * failButRollBothDown * (1 - oneDieSuccess);

				if (_proCalculator.UsePro(playerAction, r, usedSkills))
				{
					_probabilityCalculator.Calculate(pBrawler * player.ProSuccess * oneDieSuccess, r, playerAction, usedSkills | Skills.Pro);

					if (r == 0)
					{
						return;
					}

					_probabilityCalculator.Calculate(pBrawler * (1 - player.ProSuccess * oneDieSuccess) * player.LonerSuccess * oneDieSuccess, r - 1, playerAction, usedSkills | Skills.Pro);
					return;
				}

				if (r > 0)
				{
					_probabilityCalculator.Calculate(pBrawler * player.LonerSuccess * action.SuccessOnTwoDice, r - 1, playerAction, usedSkills);
					return;
				}
			}

			p *= action.Failure - failButRollBothDown;

			if (_proCalculator.UsePro(playerAction, r, usedSkills))
			{
				_probabilityCalculator.Calculate(p * player.ProSuccess * oneDieSuccess, r, playerAction, usedSkills | Skills.Pro);

				if (r > 0)
				{
					_probabilityCalculator.Calculate(p * (1 - player.ProSuccess * oneDieSuccess) * player.LonerSuccess * action.SuccessOnTwoDice, r - 1, playerAction, usedSkills | Skills.Pro);
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
