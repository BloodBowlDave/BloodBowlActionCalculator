using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Abstractions.Strategies.Blocking
{
    public interface IBlockSkillsHelper
    {
        Skills SkillsToUse(Player player, Block block, int r, Skills usedSkills, decimal successOnOneDie, decimal success);
    }
}
