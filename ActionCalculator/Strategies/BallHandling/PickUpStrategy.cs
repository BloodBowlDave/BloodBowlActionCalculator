using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class PickUpStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ICalculationContext _context;
        private readonly ID6 _d6;

        public PickUpStrategy(ICalculator calculator, IProHelper proHelper, ICalculationContext context, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _context = context;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var pickUp = playerAction.Action;

            var success = _d6.Success(1, pickUp.Roll);
            var failure = 1 - success;

            _calculator.Resolve(p * success, r, i, usedSkills);

            // CP Season 3: once per game, +1 modifier to one agility test (roll of 1 always fails)
            var cpS3Success = _context.Season == Season.Season3
                && canUseSkill(CalculatorSkills.ConsummateProfessional, usedSkills)
                && pickUp.Roll > 2
                ? 1m / 6 : 0m;
            if (cpS3Success > 0)
            {
                _calculator.Resolve(p * cpS3Success, r, i, usedSkills | CalculatorSkills.Pro);
            }
            failure -= cpS3Success;

            p *= failure * success;

            if (canUseSkill(CalculatorSkills.SureHands, usedSkills))
            {
                _calculator.Resolve(p, r, i, usedSkills);
                return;
            }

            if (_proHelper.UsePro(player, pickUp, r, usedSkills, success, success))
            {
                _calculator.Resolve(p * proSuccess, r, i, usedSkills | CalculatorSkills.Pro);
                return;
            }

            _calculator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}
