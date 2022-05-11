using ActionCalculator.Abstractions;
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

        public bool CanUseBrawler(int r, PlayerAction playerAction, Skills usedSkills)
        {
            var (player, action, _) = playerAction;

            if (!player.CanUseSkill(Skills.Brawler, usedSkills))
            {
                return false;
            }

            return r == 0
                   || action.UseBrawler
                   || action.SuccessOnOneDie >= action.Success * player.RerollSuccess;
        }

        public bool CanUseBrawlerAndPro(int r, PlayerAction playerAction, Skills usedSkills)
        {
            var player = playerAction.Player;

            if (!player.CanUseSkill(Skills.Brawler, usedSkills) || !player.CanUseSkill(Skills.Pro, usedSkills) || usedSkills.Contains(Skills.Pro))
            {
                return false;
            }

            var action = playerAction.Action;
            var successAfterBrawlerAndPro = action.SuccessOnOneDie * action.SuccessOnOneDie;
            var successAfterReroll = action.Success;

            return r == 0
                   || action.UseBrawler && action.UsePro
                   || _proHelper.CanUsePro(playerAction, r, usedSkills, successAfterBrawlerAndPro, successAfterReroll);
        }
    }
}
