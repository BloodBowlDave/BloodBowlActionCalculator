using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Blocking
{
    public class BrawlerHelper : IBrawlerHelper
    {
        private readonly IProHelper _proHelper;

        public BrawlerHelper(IProHelper proHelper)
        {
            _proHelper = proHelper;
        }

        public bool UseBrawler(Player player, Block block, int r, Skills usedSkills, decimal successOnOneDie, decimal success)
        {
            if (!player.CanUseSkill(Skills.Brawler, usedSkills))
            {
                return false;
            }

            return r == 0
                   || block.UseBrawler
                   || successOnOneDie >= success * player.LonerSuccess;
        }

        public bool UseBrawlerAndPro(Player player, Block block, int r, Skills usedSkills, decimal successOnOneDie, decimal success)
        {
            if (!player.CanUseSkill(Skills.Brawler, usedSkills) || !player.CanUseSkill(Skills.Pro, usedSkills) || usedSkills.Contains(Skills.Pro))
            {
                return false;
            }
            
            var successAfterBrawlerAndPro = successOnOneDie * successOnOneDie;
            var successAfterReroll = block.Success;

            return r == 0
                   || block.UseBrawler && block.UsePro
                   || _proHelper.UsePro(player, block, r, usedSkills, successAfterBrawlerAndPro, successAfterReroll);
        }
    }
}
