using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Abstractions.Strategies.Blocking
{
    public interface IBrawlerHelper
    {
        bool UseBrawler(Player player, Block block, int r, Skills usedSkills, decimal successOnOneDie, decimal success);
        bool UseBrawlerAndPro(Player player, Block block, int r, Skills usedSkills, decimal successOnOneDie, decimal success);
    }
}