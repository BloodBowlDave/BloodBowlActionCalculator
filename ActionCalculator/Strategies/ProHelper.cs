using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Strategies
{
    public class ProHelper : IProHelper
    {
        private readonly ICalculationContext _context;

        public ProHelper(ICalculationContext context)
        {
            _context = context;
        }

        public bool UsePro(Player player, Action action, int r, CalculatorSkills usedSkills, decimal successWithPro, decimal successWithReroll)
        {
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            // In Season 3, Consummate Professional is a +1 modifier (not a Pro reroll)
            var cpIsProReroll = _context.Season != Season.Season3;

            if (!canUseSkill(CalculatorSkills.Pro, usedSkills) &&
                !(cpIsProReroll && canUseSkill(CalculatorSkills.ConsummateProfessional, usedSkills)) &&
                !canUseSkill(CalculatorSkills.HalflingLuck, usedSkills))
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
