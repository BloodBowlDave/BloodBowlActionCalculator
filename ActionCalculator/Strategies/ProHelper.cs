using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Models;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Strategies
{
    public class ProHelper : IProHelper
    {
        public bool UsePro(Player player, Action action, int r, Skills usedSkills, decimal successWithPro, decimal successWithReroll)
        {
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            if (!canUseSkill(Skills.Pro, usedSkills) && !canUseSkill(Skills.ConsummateProfessional, usedSkills))
            {
                return false;
            }

            if (action.UsePro)
            {
                return true;
            }

            return r == 0 || proSuccess * successWithPro >= lonerSuccess * successWithReroll;
        }
    }
}
