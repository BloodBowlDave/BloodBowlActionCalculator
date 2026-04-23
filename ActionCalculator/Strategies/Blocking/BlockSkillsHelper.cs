using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Abstractions.Strategies.Blocking;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Blocking
{
    public class BlockSkillsHelper(IProHelper proHelper) : IBlockSkillsHelper
    {
        public CalculatorSkills SkillsToUse(Player player, Block block, int r, CalculatorSkills usedSkills, decimal successOnOneDie, decimal success)
        {
            var result = CalculatorSkills.None;
            if (player.CanUseSkill(CalculatorSkills.SavageBlow, usedSkills)) result |= CalculatorSkills.SavageBlow;
            if (UseBrawler(player, block, r, usedSkills, successOnOneDie, success)) result |= CalculatorSkills.Brawler;
            if (UseHatred(player, block, r, usedSkills, successOnOneDie, success)) result |= CalculatorSkills.Hatred;
            if (player.CanUseSkill(CalculatorSkills.UnstoppableMomentum, usedSkills)) result |= CalculatorSkills.UnstoppableMomentum;
            if (player.CanUseSkill(CalculatorSkills.LordOfChaos, usedSkills)) result |= CalculatorSkills.LordOfChaos;
            if (proHelper.UsePro(player, block, r, usedSkills, successOnOneDie, success)) result |= CalculatorSkills.Pro;
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
