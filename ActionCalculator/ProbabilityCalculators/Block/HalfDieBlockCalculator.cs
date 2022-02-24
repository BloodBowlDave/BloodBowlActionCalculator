using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators;
using ActionCalculator.Abstractions.ProbabilityCalculators.Block;

namespace ActionCalculator.ProbabilityCalculators.Block
{
	public class HalfDieBlockCalculator : IProbabilityCalculator
	{
		private readonly IProbabilityCalculator _probabilityCalculator;
		private readonly IProCalculator _proCalculator;
		private readonly IBrawlerCalculator _brawlerCalculator;

		public HalfDieBlockCalculator(IProbabilityCalculator probabilityCalculator, 
			IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
		{
			_probabilityCalculator = probabilityCalculator;
			_proCalculator = proCalculator;
			_brawlerCalculator = brawlerCalculator;
		}

		public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
		{
			_probabilityCalculator.Calculate(p * playerAction.Action.Success, r, playerAction, usedSkills);

			CalculateOneDiceFails(p, r, playerAction, usedSkills);
			CalculateTwoFailures(p, r, playerAction, usedSkills);
		}

		private void CalculateOneDiceFails(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
		{
			var action = playerAction.Action;
			var player = playerAction.Player;
			var oneDieSuccess = action.SuccessOnOneDie;
			var oneDiceFails = action.SuccessOnOneDie * (1 - action.SuccessOnOneDie) * 2;
			var canUseBrawler = 0m;

			if (_brawlerCalculator.UseBrawler(r, playerAction))
			{
				canUseBrawler = _brawlerCalculator.ProbabilityCanUseBrawler(action);
				_probabilityCalculator.Calculate(p * canUseBrawler * oneDieSuccess, r, playerAction, usedSkills);
			}
			
			if (_proCalculator.UsePro(playerAction, r, usedSkills))
			{
				_probabilityCalculator.Calculate(p * (oneDiceFails - canUseBrawler) * player.ProSuccess * oneDieSuccess, r, playerAction, usedSkills | Skills.Pro);
				return;
			}

			if (r > 0)
			{
				_probabilityCalculator.Calculate(p * (oneDiceFails - canUseBrawler) * player.LonerSuccess * action.Success, r + 1, playerAction, usedSkills);
			}
		}
		
		private void CalculateTwoFailures(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
		{
			var action = playerAction.Action;
			var player = playerAction.Player;
			var oneDieSuccess = action.SuccessOnOneDie;
			var twoFailures = (1 - oneDieSuccess) * (1 - oneDieSuccess);
			var canUseBrawler = 0m;

			if (_brawlerCalculator.UseBrawler(r, playerAction) && _proCalculator.UsePro(playerAction, r, usedSkills))
			{
				canUseBrawler = _brawlerCalculator.ProbabilityCanUseBrawlerAndPro(action);
				_probabilityCalculator.Calculate(p * canUseBrawler * oneDieSuccess, r, playerAction, usedSkills | Skills.Pro | Skills.Brawler);
			}

			p *= twoFailures - canUseBrawler;
			
			if (r > 0)
			{
				_probabilityCalculator.Calculate(p * player.LonerSuccess * action.Success, r - 1, playerAction, usedSkills);
			}
		}
	}
}
