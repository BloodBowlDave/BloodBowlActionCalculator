using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.BallHandling
{
    public class CatchCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public CatchCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var successNoReroll = playerAction.Action.Success;

            _calculator.Calculate(p * successNoReroll, r, playerAction, usedSkills);

            p *= successNoReroll * playerAction.Action.Failure;

            var player = playerAction.Player;
            if (player.HasSkill(Skills.Catch))
            {
                _calculator.Calculate(p, r, playerAction, usedSkills);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(p * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _calculator.Calculate(p * player.UseReroll, r - 1, playerAction, usedSkills);
            }
        }
    }
}