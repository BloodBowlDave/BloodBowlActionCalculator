using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class InjuryStrategy(ICalculator calculator, ID6 d6, ICalculationContext context) : IActionStrategy
    {
        private const CalculatorSkills SkillsAffectingInjury = CalculatorSkills.Ram | CalculatorSkills.BrutalBlock | CalculatorSkills.MightyBlow | CalculatorSkills.Slayer;

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = (Injury) playerAction.Action;
            var modifier = GetModifier(player, usedSkills);
            var success = d6.Success(2, action.Roll - modifier);

            calculator.Resolve(p * success, r, i, usedSkills);

            if (player.CanUseSkill(CalculatorSkills.ToxinConnoisseur, usedSkills) && context.PreviousActionType == ActionType.Stab)
            {
                var successWithTC = d6.Success(2, action.Roll - modifier - 1);
                calculator.Resolve(p * (successWithTC - success), r, i, usedSkills | CalculatorSkills.ToxinConnoisseur);
            }

            if (player.CanUseSkill(CalculatorSkills.SavageMauling, usedSkills))
            {
                calculator.Resolve(p * (1 - success) * success, r, i, usedSkills);
            }
        }

        private int GetModifier(Player player, CalculatorSkills usedSkills)
        {
            if (context.PreviousActionType != ActionType.ArmourBreak)
            {
                return 0;
            }

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