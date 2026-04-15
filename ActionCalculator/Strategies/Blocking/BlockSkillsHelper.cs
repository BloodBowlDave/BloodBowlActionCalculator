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

        public CalculatorSkills SkillsToUse(Player player, Block block, int r, CalculatorSkills usedSkills, decimal successOnOneDie, decimal success)
        {
            var result = CalculatorSkills.None;
            if (UseBrawler(player, block, r, usedSkills, successOnOneDie, success)) result |= CalculatorSkills.Brawler;
            if (UseHatred(player, block, r, usedSkills, successOnOneDie, success)) result |= CalculatorSkills.Hatred;
            if (_proHelper.UsePro(player, block, r, usedSkills, successOnOneDie, success)) result |= CalculatorSkills.Pro;
            return result;
        }

        private bool UseBrawler(Player player, Block block, int r, CalculatorSkills usedSkills, decimal successOnOneDie, decimal success)
        {
            if (!player.CanUseSkill(CalculatorSkills.Brawler, usedSkills))
            {
                return false;
            }

            return r == 0
                   || block.UseBrawler
                   || successOnOneDie >= success * player.LonerSuccess();
        }

        private bool UseHatred(Player player, Block block, int r, CalculatorSkills usedSkills, decimal successOnOneDie, decimal success)
        {
            if (!player.CanUseSkill(CalculatorSkills.Hatred, usedSkills))
            {
                return false;
            }

            return (r == 0 && block.UseHatred)
                   || (r != 0 && successOnOneDie >= success * player.LonerSuccess());
        }
    }
}
