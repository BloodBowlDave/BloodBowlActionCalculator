using ActionCalculator.Abstractions;

namespace ActionCalculator
{
	public class ProCalculator : IProCalculator
	{
		public bool UsePro(PlayerAction playerAction, int r, Skills usedSkills)
		{
			var player = playerAction.Player;

			if (!player.HasSkill(Skills.Pro) || usedSkills.HasFlag(Skills.Pro))
			{
				return false;
			}

			var action = playerAction.Action;

			if (action.UseProBeforeReroll)
			{
				return true;
			}

			return r == 0 || player.ProSuccess > player.LonerSuccess;
		}
	}
}
