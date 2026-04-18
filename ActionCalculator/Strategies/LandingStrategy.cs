using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
    public class LandingStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ICalculationContext _context;
        private readonly ID6 _d6;

        public LandingStrategy(ICalculator calculator, IProHelper proHelper, ICalculationContext context, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _context = context;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var landing = playerAction.Action;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var roll = nonCriticalFailure ? landing.Roll + 1 : landing.Roll;
            var success = _d6.Success(1, roll);
            var failure = 1 - success;

            _calculator.Resolve(p * success, r, i, usedSkills);

            // CP Season 3: once per game, +1 modifier to one agility test (roll of 1 always fails)
            var cpS3Success = _context.Season == Season.Season3
                && canUseSkill(CalculatorSkills.ConsummateProfessional, usedSkills)
                && roll > 2
                ? 1m / 6 : 0m;
            if (cpS3Success > 0)
            {
                _calculator.Resolve(p * cpS3Success, r, i, usedSkills | CalculatorSkills.Pro);
            }
            failure -= cpS3Success;

            if (_proHelper.UsePro(player, landing, r, usedSkills, success, success))
            {
                _calculator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | CalculatorSkills.Pro);
                return;
            }

            _calculator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills);
        }
    }
}
