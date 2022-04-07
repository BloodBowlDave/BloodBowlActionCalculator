using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
	public class ProCalculator : IProCalculator
	{
		public bool UsePro(PlayerAction playerAction, int r, Skills usedSkills, decimal? successOnOneDie, decimal? successAfterReroll)
		{
			var player = playerAction.Player;

			if (!player.HasSkill(Skills.Pro) || usedSkills.HasFlag(Skills.Pro))
			{
				return false;
			}

			var action = playerAction.Action;

			if (action.UsePro)
			{
				return true;
			}
			
            return r == 0 || player.ProSuccess * (successOnOneDie ?? action.SuccessOnOneDie) >= player.LonerSuccess * (successAfterReroll ?? action.Success);
		}
	}
}
