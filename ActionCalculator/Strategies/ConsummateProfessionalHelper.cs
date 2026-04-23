using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
    internal class ConsummateProfessionalHelper(
        ICalculationContext context) : IConsummateProfessionalHelper
    {
        public decimal GetAgilityRollSuccess(
            CalculatorSkills usedSkills, 
            int roll, 
            Func<CalculatorSkills, CalculatorSkills, bool> canUseSkill) 
            => context.Season == Season.Season3
                && canUseSkill(CalculatorSkills.ConsummateProfessional, usedSkills)
                && roll > 2
                ? 1m / 6 : 0m;
    }
}
