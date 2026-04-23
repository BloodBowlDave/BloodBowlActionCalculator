using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Strategies
{
    public class ProHelper(ICalculationContext context) : IProHelper
    {
        public bool UsePro(Player player, Action action, int r, CalculatorSkills usedSkills, decimal successWithPro, decimal successWithReroll)
        {
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var consummateProfessionalIsProReroll = context.Season == Season.Season2;

            if (!canUseSkill(CalculatorSkills.Pro, usedSkills) &&
                !(consummateProfessionalIsProReroll && canUseSkill(CalculatorSkills.ConsummateProfessional, usedSkills)) &&
                !canUseSkill(CalculatorSkills.HalflingLuck, usedSkills) &&
                !canUseSkill(CalculatorSkills.ThinkingMansTroll, usedSkills) &&
                !canUseSkill(CalculatorSkills.BoundingLeap, usedSkills))
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
