﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
	public class ChainsawStrategy : IActionStrategy
	{
		private readonly ICalculator _calculator;
		private readonly ID6 _d6;

		public ChainsawStrategy(ICalculator calculator, ID6 d6)
		{
			_calculator = calculator;
			_d6 = d6;
		}

		public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
		{
			var action = playerAction.Action;
			var success = _d6.Success(2, action.Roll);

			_calculator.Resolve(p * success, r, i, usedSkills);
			_calculator.Resolve(p * (1 - success), r, i, usedSkills, true);
		}
	}
}