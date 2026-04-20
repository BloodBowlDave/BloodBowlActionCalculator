using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class InjuryStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _d6;
        private readonly ICalculationContext _context;
        private const CalculatorSkills SkillsAffectingInjury = CalculatorSkills.Ram | CalculatorSkills.BrutalBlock | CalculatorSkills.MightyBlow | CalculatorSkills.Slayer;

        public InjuryStrategy(ICalculator calculator, ID6 d6, ICalculationContext context)
        {
            _calculator = calculator;
            _d6 = d6;
            _context = context;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = (Injury) playerAction.Action;
            var modifier = GetModifier(player, usedSkills);
            var success = _d6.Success(2, action.Roll - modifier);

            _calculator.Resolve(p * success, r, i, usedSkills);

            if (player.CanUseSkill(CalculatorSkills.ToxinConnoisseur, usedSkills) && _context.PreviousActionType == ActionType.Stab)
            {
                var successWithTC = _d6.Success(2, action.Roll - modifier - 1);
                _calculator.Resolve(p * (successWithTC - success), r, i, usedSkills | CalculatorSkills.ToxinConnoisseur);
            }

            if (player.CanUseSkill(CalculatorSkills.SavageMauling, usedSkills))
            {
                _calculator.Resolve(p * (1 - success) * success, r, i, usedSkills);
            }
        }

        private static int GetModifier(Player player, CalculatorSkills usedSkills)
        {
            var modifier = 0;

            foreach (var skill in SkillsAffectingInjury.ToEnumerable(CalculatorSkills.None)
                         .Where(x => player.CanUseSkill(x, usedSkills) && !usedSkills.Contains(x)))
            {
                switch (skill)
                {
                    case CalculatorSkills.MightyBlow:
                        modifier += player.MightyBlowValue;
                        break;
                    case CalculatorSkills.Ram:
                    case CalculatorSkills.Slayer:
                    case CalculatorSkills.BrutalBlock:
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