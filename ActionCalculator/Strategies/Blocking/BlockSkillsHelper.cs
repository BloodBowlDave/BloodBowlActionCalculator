using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Abstractions.Strategies.Blocking;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Blocking
{
    public class BlockSkillsHelper : IBlockSkillsHelper
    {
        private readonly IProHelper _proHelper;

        public BlockSkillsHelper(IProHelper proHelper)
        {
            _proHelper = proHelper;
        }

        public Skills SkillsToUse(Player player, Block block, int r, Skills usedSkills, decimal successOnOneDie, decimal success)
        {
            var result = Skills.None;
            if (UseBrawler(player, block, r, usedSkills, successOnOneDie, success)) result |= Skills.Brawler;
            if (UseHatred(player, block, r, usedSkills, successOnOneDie, success)) result |= Skills.Hatred;
            if (_proHelper.UsePro(player, block, r, usedSkills, successOnOneDie, success)) result |= Skills.Pro;
            return result;
        }

        private bool UseBrawler(Player player, Block block, int r, Skills usedSkills, decimal successOnOneDie, decimal success)
        {
            if (!player.CanUseSkill(Skills.Brawler, usedSkills))
            {
                return false;
            }

            return r == 0
                   || block.UseBrawler
                   || successOnOneDie >= success * player.LonerSuccess();
        }

        private bool UseHatred(Player player, Block block, int r, Skills usedSkills, decimal successOnOneDie, decimal success)
        {
            if (!player.CanUseSkill(Skills.Hatred, usedSkills))
            {
                return false;
            }

            return (r == 0 && block.UseHatred)
                   || (r != 0 && successOnOneDie >= success * player.LonerSuccess());
        }
    }
}
