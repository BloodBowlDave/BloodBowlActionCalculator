using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Calculators
{
    public class ProCalculator : IProCalculator
    {
        public bool UsePro(PlayerAction playerAction, int r, Skills usedSkills, decimal? successOnOneDie, decimal? successAfterReroll)
        {
            var player = playerAction.Player;

            if (!player.HasSkill(Skills.Pro) && !player.HasSkill(Skills.ConsummateProfessional) || usedSkills.Contains(Skills.Pro))
            {
                return false;
            }

            var action = playerAction.Action;

            if (action.UsePro)
            {
                return true;
            }

            return r == 0 || player.ProSuccess * (successOnOneDie ?? action.SuccessOnOneDie) >= player.UseReroll * (successAfterReroll ?? action.Success);
        }
    }
}
