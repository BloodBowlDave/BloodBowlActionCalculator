using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Abstractions.Strategies.Blocking
{
    public interface IBlockSkillsHelper
    {
        CalculatorSkills SkillsToUse(Player player, Block block, int r, CalculatorSkills usedSkills, decimal successOnOneDie, decimal success);
    }
}
