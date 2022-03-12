using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.BallHandling
{
    public class InterceptionCalculator : ICalculator
    {
        private readonly ICalculator _calculator;

        public InterceptionCalculator(ICalculator calculator)
        {
            _calculator = calculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            
            var interceptionFailure = nonCriticalFailure 
                ? 1 - (7m - (action.OriginalRoll - 1).ThisOrMinimum(2).ThisOrMaximum(6)) / 6
                : action.Failure;
            
            if (player.HasSkill(Skills.CloudBurster))
            {
                _calculator.Calculate(p * (1 - (1 - interceptionFailure) * (1 - interceptionFailure)), r, playerAction, usedSkills, nonCriticalFailure);
                return;
            }
            
            _calculator.Calculate(p * interceptionFailure, r, playerAction, usedSkills, nonCriticalFailure);
        }
    }
}