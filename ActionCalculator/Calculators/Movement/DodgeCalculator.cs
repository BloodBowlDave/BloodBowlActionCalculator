using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Movement
{
    public class DodgeCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public DodgeCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
	        _calculator = calculator;
	        _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var affectedByDivingTackle = action.AffectedByDivingTackle && !usedSkills.HasFlag(Skills.DivingTackle);
            var affectedByBreakTackle = player.HasSkill(Skills.BreakTackle) && !usedSkills.HasFlag(Skills.BreakTackle) && player.BreakTackleValue > 0;
            var successAfterModifiers = SuccessAfterModifiers(playerAction, affectedByDivingTackle, affectedByBreakTackle);

            var success = action.Success;
            var useDivingTackle = 0m;
            if (affectedByDivingTackle)
            {
                useDivingTackle = success - successAfterModifiers;
                success -= useDivingTackle;
            }

            _calculator.Calculate(p * success, r, playerAction, usedSkills);

            var failure = action.Failure;
            var useBreakTackle = 0m;
            if (affectedByBreakTackle)
            {
                useBreakTackle = successAfterModifiers - success;
                failure -= useBreakTackle;
                _calculator.Calculate(p * useBreakTackle, r, playerAction, usedSkills | Skills.BreakTackle);
            }

            if (player.HasSkill(Skills.Dodge) && !usedSkills.HasFlag(Skills.Dodge))
            {
                CalculateDodgeReroll(p, r, playerAction, usedSkills | Skills.Dodge, failure, success, useBreakTackle, useDivingTackle);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                CalculateDodgeReroll(p * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro, failure, success, useBreakTackle, useDivingTackle);
                return;
            }

            if (r == 0)
            {
                return;
            }
            
            CalculateDodgeReroll(p * player.LonerSuccess, r - 1, playerAction, usedSkills, failure, success, useBreakTackle, useDivingTackle);
        }

        private static decimal SuccessAfterModifiers(PlayerAction playerAction, bool affectedByDivingTackle, bool affectedByBreakTackle) => 
            (7m - (playerAction.Action.OriginalRoll + (affectedByDivingTackle ? 2 : 0) - (affectedByBreakTackle ? playerAction.Player.BreakTackleValue : 0))
                .ThisOrMinimum(2).ThisOrMaximum(6)) / 6;

        private void CalculateDodgeReroll(decimal p, int r, PlayerAction playerAction, Skills usedSkills, 
            decimal failure, decimal success, decimal useBreakTackle, decimal useDivingTackle)
        {
            _calculator.Calculate(p * failure * success, r, playerAction, usedSkills);
            _calculator.Calculate(p * failure * useBreakTackle, r, playerAction, usedSkills | Skills.BreakTackle);
            _calculator.Calculate(p * useDivingTackle * success, r, playerAction, usedSkills | Skills.DivingTackle);
            _calculator.Calculate(p * useDivingTackle * useBreakTackle, r, playerAction, usedSkills | Skills.DivingTackle | Skills.BreakTackle);
        }
    }
}