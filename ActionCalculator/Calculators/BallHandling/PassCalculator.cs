using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.BallHandling
{
    public class PassCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public PassCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var success = action.Success;

            _calculator.Calculate(p * success, r, playerAction, usedSkills);

            var inaccuratePass = action.NonCriticalFailure;
            var rerollInaccuratePass = action.RerollNonCriticalFailure;
            var accuratePassAfterFailure = p * (action.Failure + (rerollInaccuratePass ? inaccuratePass : 0)) * success;
            var inaccuratePassAfterFailure = p * (action.Failure + (rerollInaccuratePass ? inaccuratePass : 0)) * inaccuratePass;
            var inaccuratePassWithoutReroll = p * (rerollInaccuratePass ? 0m : inaccuratePass);

            if (player.HasSkill(Skills.Pass) && action.ActionType == ActionType.Pass || player.HasSkill(Skills.TheBallista))
            {
                _calculator.Calculate(accuratePassAfterFailure, r, playerAction, usedSkills);
                _calculator.Calculate(inaccuratePassWithoutReroll + inaccuratePassAfterFailure, r, playerAction, usedSkills, true);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(inaccuratePassWithoutReroll, r, playerAction, usedSkills, true);

                usedSkills |= Skills.Pro;
                _calculator.Calculate(player.ProSuccess * accuratePassAfterFailure, r, playerAction, usedSkills);
                _calculator.Calculate(player.ProSuccess * inaccuratePassAfterFailure, r, playerAction, usedSkills, true);
                return;
            }

            if (r > 0 && rerollInaccuratePass)
            {
                _calculator.Calculate(player.UseReroll * accuratePassAfterFailure, r - 1, playerAction, usedSkills);
                _calculator.Calculate(player.UseReroll * inaccuratePassAfterFailure, r - 1, playerAction, usedSkills, true);
                return;
            }

            _calculator.Calculate(p * inaccuratePass, r, playerAction, usedSkills, true);
        }
    }
}