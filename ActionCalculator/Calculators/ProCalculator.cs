using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class ProCalculator : IProCalculator
    {
        public bool UsePro(PlayerAction playerAction, int r, Skills usedSkills, decimal? successOnOneDie, decimal? successAfterReroll)
        {
            var ((rerollSuccess, proSuccess, hasSkill), action, _) = playerAction;
            var success = action.Success;

            if (!hasSkill(Skills.Pro, usedSkills) && !hasSkill(Skills.ConsummateProfessional, usedSkills))
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
