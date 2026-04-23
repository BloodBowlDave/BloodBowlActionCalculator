using ActionCalculator.Models;

namespace ActionCalculator.Abstractions.Strategies
{
    public interface IConsummateProfessionalHelper
    {
        decimal GetAgilityRollSuccess(
            CalculatorSkills usedSkills,
            int roll,
            Func<CalculatorSkills, CalculatorSkills, bool> canUseSkill);
    }
}
