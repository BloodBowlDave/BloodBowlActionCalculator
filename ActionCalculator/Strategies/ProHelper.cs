using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies
{
    public class ProHelper : IProHelper
    {
        public bool UsePro(PlayerAction playerAction, int r, Skills usedSkills, decimal? successOnOneDie, decimal? successAfterReroll)
        {
            var ((rerollSuccess, proSuccess, canUseSkill), action, _) = playerAction;
            var success = action.Success;

            if (!canUseSkill(Skills.Pro, usedSkills) && !canUseSkill(Skills.ConsummateProfessional, usedSkills))
            {
                return false;
            }

            if (action.UsePro)
            {
                return true;
            }

            return r == 0 || proSuccess * (successOnOneDie ?? action.SuccessOnOneDie) >= rerollSuccess * (successAfterReroll ?? success);
        }
    }
}
