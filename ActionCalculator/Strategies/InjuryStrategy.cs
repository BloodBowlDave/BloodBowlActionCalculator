using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class InjuryStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _iD6;
        private const Skills SkillsAffectingInjury = Skills.Ram | Skills.BrutalBlock | Skills.MightyBlow | Skills.Slayer | Skills.DirtyPlayer;


        public InjuryStrategy(IActionMediator actionMediator, ID6 iD6)
        {
            _actionMediator = actionMediator;
            _iD6 = iD6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = (Injury) playerAction.Action;
            var i = playerAction.Index;
            var modifier = GetModifier(player, usedSkills);
            var success = _iD6.Success(2, action.Roll - modifier);

            _actionMediator.Resolve(p * success, r, i, usedSkills, nonCriticalFailure);

            if (player.CanUseSkill(Skills.SavageMauling, usedSkills))
            {
                _actionMediator.Resolve(p * (1 - success) * success, r, i, usedSkills, nonCriticalFailure);
            }
        }

        private static int GetModifier(Player player, Skills usedSkills)
        {
            var modifier = 0;

            foreach (var skill in SkillsAffectingInjury.ToEnumerable(Skills.None)
                         .Where(x => player.CanUseSkill(x, usedSkills) && !usedSkills.Contains(x)))
            {
                switch (skill)
                {
                    case Skills.DirtyPlayer:
                        return player.DirtyPlayerValue;
                    case Skills.MightyBlow:
                        modifier += player.MightyBlowValue;
                        break;
                    case Skills.Ram:
                    case Skills.Slayer:
                    case Skills.BrutalBlock:
                        modifier++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(skill));
                }
            }

            return modifier;
        }
    }
}