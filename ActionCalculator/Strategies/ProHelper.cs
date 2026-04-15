using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Strategies
{
    public class ProHelper : IProHelper
    {
        public bool UsePro(Player player, Action action, int r, CalculatorSkills usedSkills, decimal successWithPro, decimal successWithReroll)
        {
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            if (!canUseSkill(CalculatorSkills.Pro, usedSkills) && !canUseSkill(CalculatorSkills.ConsummateProfessional, usedSkills))
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
