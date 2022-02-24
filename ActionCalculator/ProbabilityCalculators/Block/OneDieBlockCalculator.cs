﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators;
using ActionCalculator.Abstractions.ProbabilityCalculators.Block;

namespace ActionCalculator.ProbabilityCalculators.Block
{
	public class OneDieBlockCalculator : IProbabilityCalculator
	{
		private readonly IProbabilityCalculator _probabilityCalculator;
		private readonly IProCalculator _proCalculator;
		private readonly IBrawlerCalculator _brawlerCalculator;
		
		public OneDieBlockCalculator(IProbabilityCalculator probabilityCalculator, 
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

			_probabilityCalculator.Calculate(p * success, r, playerAction, usedSkills);

			var failButRollBothDown = 0m;

			if (_brawlerCalculator.UseBrawler(r, playerAction))
			{
				failButRollBothDown = _brawlerCalculator.ProbabilityCanUseBrawler(action);
				_probabilityCalculator.Calculate(p * failButRollBothDown * success, r, playerAction, usedSkills);
			}

			p *= (action.Failure - failButRollBothDown) * success;

			if (_proCalculator.UsePro(playerAction, r, usedSkills))
			{
				_probabilityCalculator.Calculate(p * player.ProSuccess, r, playerAction, usedSkills);
				return;
			}

			if (r > 0)
			{
				_probabilityCalculator.Calculate(p * player.LonerSuccess, r - 1, playerAction, usedSkills);
			}
		}
	}
}
