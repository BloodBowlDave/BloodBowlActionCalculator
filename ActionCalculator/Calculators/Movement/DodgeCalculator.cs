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
            var success = action.Success;
            var failure = action.Failure;
            var triggerDivingTackle = 0m;

            if (action.AffectedByDivingTackle && !usedSkills.HasFlag(Skills.DivingTackle))
            {
                triggerDivingTackle = success - (7m - (action.OriginalRoll + 2).ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
                success -= triggerDivingTackle;
            }

            _calculator.Calculate(p * success, r, playerAction, usedSkills);
            
            if (player.HasSkill(Skills.Dodge) && !usedSkills.HasFlag(Skills.Dodge))
            {
                _calculator.Calculate(p * failure * success, r, playerAction, usedSkills | Skills.Dodge);
                _calculator.Calculate(p * triggerDivingTackle * action.Success, r, playerAction, usedSkills | Skills.Dodge | Skills.DivingTackle);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
	            _calculator.Calculate(p * failure * success * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro);
	            _calculator.Calculate(p * triggerDivingTackle * action.Success * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro | Skills.DivingTackle);
                return;
            }

            if (r == 0)
            {
                return;
            }

            _calculator.Calculate(p * failure * success * player.LonerSuccess, r - 1, playerAction, usedSkills);
            _calculator.Calculate(p * triggerDivingTackle * action.Success * player.LonerSuccess, r - 1, playerAction, usedSkills | Skills.DivingTackle);
        }
    }
}