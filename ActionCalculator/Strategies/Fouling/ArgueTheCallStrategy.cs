using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Fouling
{
    public class ArgueTheCallStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _d6;

        public ArgueTheCallStrategy(ICalculator calculator, ID6 d6)
        {
            _calculator = calculator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            if (nonCriticalFailure)
            {
                var argueTheCall = (ArgueTheCall)playerAction.Action;
                var success = _d6.Success(1, argueTheCall.Numerator);
                var failure = 1 - success - 1m / 6;

                _calculator.Resolve(p * success, r, i, usedSkills);
                _calculator.Resolve(p * failure, r, i, usedSkills, true);
                return;
            }

            _calculator.Resolve(p, r, i, usedSkills);
        }
    }
}