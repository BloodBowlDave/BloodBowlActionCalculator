using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Calculators.Blocking
{
	public class ThirdDieBlockCalculator : ICalculator
	{
		private readonly ICalculator _calculator;
		private readonly IProCalculator _proCalculator;
		private readonly IBrawlerCalculator _brawlerCalculator;

		public ThirdDieBlockCalculator(ICalculator calculator, 
			IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
		{
			_calculator = calculator;
			_proCalculator = proCalculator;
			_brawlerCalculator = brawlerCalculator;
		}

		public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
		{
			_calculator.Calculate(p * playerAction.Action.Success, r, playerAction, usedSkills);

            CalculateOneDiceFails(p, r, playerAction, usedSkills);
            CalculateTwoDiceFail(p, r, playerAction, usedSkills);
            CalculateThreeDiceFail(p, r, playerAction, usedSkills);
		}

        private void CalculateOneDiceFails(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
		{
			var action = playerAction.Action;
            var player = playerAction.Player;
            var oneDieSuccess = action.SuccessOnOneDie;
            var oneDiceFails = oneDieSuccess * oneDieSuccess * (1 - oneDieSuccess) * 3;
            var canUseBrawler = 0m;

            if (_brawlerCalculator.UseBrawler(r, playerAction))
            {
                canUseBrawler = _brawlerCalculator.ProbabilityCanUseBrawler(action);
                _calculator.Calculate(p * canUseBrawler * oneDieSuccess, r, playerAction, usedSkills);
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(p * (oneDiceFails - canUseBrawler) * player.ProSuccess * oneDieSuccess, r, playerAction, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _calculator.Calculate(p * (oneDiceFails - canUseBrawler) * player.LonerSuccess * action.Success, r - 1, playerAction, usedSkills);
            }
		}

        private void CalculateTwoDiceFail(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
		{
			var action = playerAction.Action;
            var player = playerAction.Player;
            var oneDieSuccess = action.SuccessOnOneDie;
            var twoFailures = oneDieSuccess * (1 - oneDieSuccess) * (1 - oneDieSuccess) * 3;
            var canUseBrawler = 0m;

            if (_brawlerCalculator.UseBrawler(r, playerAction) && _proCalculator.UsePro(playerAction, r, usedSkills))
            {
                canUseBrawler = _brawlerCalculator.ProbabilityCanUseBrawlerAndPro(action);
                _calculator.Calculate(p * canUseBrawler * oneDieSuccess, r, playerAction, usedSkills | Skills.Pro);
            }

            p *= twoFailures - canUseBrawler;

            if (r > 0)
            {
                _calculator.Calculate(p * player.LonerSuccess * action.Success, r - 1, playerAction, usedSkills);
            }
		}

        private void CalculateThreeDiceFail(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
        {
            if (r == 0)
            {
                return;
            }

            var action = playerAction.Action;
            var player = playerAction.Player;
            var oneDieSuccess = action.SuccessOnOneDie;
            var threeFailures = (1 - oneDieSuccess) * (1 - oneDieSuccess) * (1 - oneDieSuccess);

            _calculator.Calculate(p * threeFailures * player.LonerSuccess * action.Success, r - 1, playerAction, usedSkills);
        }
    }
}
