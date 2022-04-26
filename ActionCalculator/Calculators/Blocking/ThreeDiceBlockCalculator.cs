using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Calculators.Blocking
{
	public class ThreeDiceBlockCalculator : ICalculator
	{
		private readonly ICalculator _calculator;
		private readonly IProCalculator _proCalculator;
		private readonly IBrawlerCalculator _brawlerCalculator;

		public ThreeDiceBlockCalculator(ICalculator calculator,
			IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
		{
			_calculator = calculator;
			_proCalculator = proCalculator;
			_brawlerCalculator = brawlerCalculator;
		}

		public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
		{
			var player = playerAction.Player;
			var action = playerAction.Action;
			var success = action.Success;
			var oneDieSuccess = action.SuccessOnOneDie;

			_calculator.Calculate(p * success, r, playerAction, usedSkills);

			var failButRollBothDown = 0m;

			if (_brawlerCalculator.UseBrawler(r, playerAction))
			{
				failButRollBothDown = _brawlerCalculator.ProbabilityCanUseBrawler(action);
				_calculator.Calculate(p * failButRollBothDown * oneDieSuccess, r, playerAction, usedSkills);

				var pBrawler = p * failButRollBothDown * (1 - oneDieSuccess);

				if (_proCalculator.UsePro(playerAction, r, usedSkills, oneDieSuccess, success))
                {
                    usedSkills |= Skills.Pro;
					_calculator.Calculate(pBrawler * player.ProSuccess * oneDieSuccess, r, playerAction, usedSkills);

					if (r == 0)
					{
						return;
					}

					_calculator.Calculate(pBrawler * (1 - player.ProSuccess * oneDieSuccess) * player.UseReroll * oneDieSuccess, r - 1, playerAction, usedSkills);
					return;
				}

				if (r > 0)
				{
					_calculator.Calculate(pBrawler * player.UseReroll * action.SuccessOnTwoDice, r - 1, playerAction, usedSkills);
					return;
				}
			}

			p *= action.Failure - failButRollBothDown;

			if (_proCalculator.UsePro(playerAction, r, usedSkills, oneDieSuccess, success))
            {
                usedSkills |= Skills.Pro;
				_calculator.Calculate(p * player.ProSuccess * oneDieSuccess, r, playerAction, usedSkills);

				if (r > 0)
				{
					_calculator.Calculate(p * (1 - player.ProSuccess * oneDieSuccess) * player.UseReroll * action.SuccessOnTwoDice, r - 1, playerAction, usedSkills);
					return;
				}
			}

			if (r > 0)
			{
				_calculator.Calculate(p * player.UseReroll * success, r - 1, playerAction, usedSkills);
			}
		}
	}
}
