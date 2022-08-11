using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
    public class LandingStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public LandingStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _d6 = d6;
        }
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var landing = playerAction.Action;
            var (lonerSuccess, proSuccess, _) = player;

            var success = _d6.Success(1, nonCriticalFailure ? landing.Roll + 1 : landing.Roll);
            var failure = 1 - success;

            _calculator.Resolve(p * success, r, i, usedSkills);

            if (_proHelper.UsePro(player, landing, r, usedSkills, success, success))
            {
                _calculator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _calculator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills);
        }
    }
}
