using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class NonRerollableStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _d6;

        public NonRerollableStrategy(ICalculator calculator, ID6 d6)
        {
            _calculator = calculator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = (NonRerollable) playerAction.Action;
            var roll = action.Numerator;
            var denominator = action.Denominator;

            var success = denominator == 12 ? _d6.Success(2, roll) : (decimal)roll / denominator;
            
            var failure = 1 - success;

            _calculator.Resolve(p * success, r, i, usedSkills, nonCriticalFailure);

            if (player.CanUseSkill(Skills.WhirlingDervish, usedSkills) && !usedSkills.Contains(Skills.WhirlingDervish) && denominator == 6)
            {
                _calculator.Resolve(p * failure * success, r, i, usedSkills | Skills.WhirlingDervish);
            }
        }
    }
}