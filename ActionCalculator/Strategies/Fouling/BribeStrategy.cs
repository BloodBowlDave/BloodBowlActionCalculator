using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.Fouling
{
    public class BribeStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _d6;

        public BribeStrategy(ICalculator calculator, ID6 d6)
        {
            _calculator = calculator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var success = _d6.Success(1, 2);
            var failure = 1 - success;

            if (nonCriticalFailure)
            {
                _calculator.Resolve(p * success, r, i, usedSkills);
                _calculator.Resolve(p * failure, r, i, usedSkills, true);
                return;
            }

            _calculator.Resolve(p, r, i, usedSkills);
        }
    }
}