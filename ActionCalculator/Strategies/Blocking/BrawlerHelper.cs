using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.Strategies.Blocking
{
    public class BrawlerHelper : IBrawlerHelper
    {
        private readonly IProHelper _proHelper;

        public BrawlerHelper(IProHelper proHelper)
        {
            _proHelper = proHelper;
        }

        public decimal UseBrawler(Action action) =>
            action.NumberOfDice switch
            {
                -3 => action.NumberOfSuccessfulResults switch
                {
                    1 => 3m / 216,
                    2 => 12m / 216,
                    4 => 48m / 216,
                    _ => 0
                },
                -2 => action.NumberOfSuccessfulResults switch
                {
                    1 => 2m / 36,
                    2 => 4m / 36,
                    4 => 8m / 36,
                    _ => 0
                },
                1 => action.NumberOfSuccessfulResults switch
                {
                    1 => 1m / 6,
                    2 => 1m / 6,
                    4 => 1m / 6,
                    _ => 0
                },
                2 => action.NumberOfSuccessfulResults switch
                {
                    1 => 9m / 36,
                    2 => 7m / 36,
                    4 => 3m / 36,
                    _ => 0
                },
                3 => action.NumberOfSuccessfulResults switch
                {
                    1 => 61m / 216,
                    2 => 37m / 216,
                    4 => 7m / 216,
                    _ => 0
                },
                _ => throw new ArgumentOutOfRangeException(nameof(action.NumberOfDice))
            };

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

        public bool UseBrawlerAndPro(int r, PlayerAction playerAction, Skills usedSkills)
        {
            var player = playerAction.Player;

            if (!player.CanUseSkill(Skills.Brawler, usedSkills) || !player.CanUseSkill(Skills.Pro, usedSkills) || usedSkills.Contains(Skills.Pro))
            {
                return false;
            }

            var action = playerAction.Action;
            var successAfterBrawlerAndPro = action.SuccessOnOneDie * player.ProSuccess * action.SuccessOnOneDie;
            var successAfterReroll = action.Success * player.RerollSuccess;

            return r == 0
                   || action.UseBrawler && action.UsePro
                   || _proHelper.CanUsePro(playerAction, r, usedSkills, successAfterBrawlerAndPro, successAfterReroll);
        }

        public decimal ProbabilityCanUseBrawlerAndPro(Action action) =>
            action.NumberOfDice switch
            {
                -3 => action.NumberOfSuccessfulResults switch
                {
                    1 => 19m / 216,
                    2 => 34m / 216,
                    4 => 28m / 216,
                    _ => 0
                },
                -2 => action.NumberOfSuccessfulResults switch
                {
                    1 => 9m / 36,
                    2 => 7m / 36,
                    4 => 3m / 36,
                    _ => 0
                },
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
