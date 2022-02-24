using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators;

namespace ActionCalculator.ProbabilityCalculators
{
	public class CatchCalculator : IProbabilityCalculator
	{
		private readonly IProbabilityCalculator _probabilityCalculator;
		private readonly IProCalculator _proCalculator;
		
		public CatchCalculator(IProbabilityCalculator probabilityCalculator, IProCalculator proCalculator)
		{
			_probabilityCalculator = probabilityCalculator;
			_proCalculator = proCalculator;
		}

		public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
		{
			var successNoReroll = playerAction.Action.Success;

			_probabilityCalculator.Calculate(p * successNoReroll, r, playerAction, usedSkills);

			p *= successNoReroll * playerAction.Action.Failure;
			
			var player = playerAction.Player;
			if (player.HasSkill(Skills.Catch))
			{
				_probabilityCalculator.Calculate(p, r, playerAction, usedSkills);
				return;
			}
			
			if (_proCalculator.UsePro(playerAction, r, usedSkills))
			{
				_probabilityCalculator.Calculate(p * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro);
				return;
			}
			
			if (r > 0)
			{
				_probabilityCalculator.Calculate(p * player.LonerSuccess, r - 1, playerAction, usedSkills);
			}
		}
	}
}