using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Calculators
{
    public class InjuryCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly ITwoD6 _twoD6;
        private const Skills SkillsAffectingInjury = Skills.Ram | Skills.BrutalBlock | Skills.MightyBlow | Skills.Slayer | Skills.DirtyPlayer;


        public InjuryCalculator(ICalculator calculator, ITwoD6 twoD6)
        {
            _calculator = calculator;
            _twoD6 = twoD6;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var modifier = GetModifier(player, usedSkills);
            var success = _twoD6.Success(action.OriginalRoll - modifier);

            _calculator.Calculate(p * success, r, playerAction, usedSkills, nonCriticalFailure);

            if (player.HasSkill(Skills.SavageMauling))
            {
                _calculator.Calculate(p * (1 - success) * success, r, playerAction, usedSkills, nonCriticalFailure);
            }
        }

        private static int GetModifier(Player player, Skills usedSkills)
        {
            var modifier = 0;

            foreach (var skill in SkillsAffectingInjury.ToEnumerable(Skills.None)
                         .Where(x => player.HasSkill(x) && !usedSkills.Contains(x)))
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