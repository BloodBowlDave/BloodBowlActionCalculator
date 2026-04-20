using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Movement
{
    public class LeapStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ICalculationContext _context;
        private readonly ID6 _d6;

        public LeapStrategy(ICalculator calculator, IProHelper proHelper, ICalculationContext context, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _context = context;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = (Models.Actions.Leap)playerAction.Action;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var useDivingTackle = action.UseDivingTackle && !usedSkills.Contains(CalculatorSkills.DivingTackle);

            var success = _d6.Success(1, action.Roll);
            var failure = 1 - success;

            var failureWithDivingTackle = 0m;
            if (useDivingTackle)
            {
                var successWithDivingTackle = _d6.Success(1, action.Roll + 2);
                failureWithDivingTackle = success - successWithDivingTackle;
                success = successWithDivingTackle;
            }

            _calculator.Resolve(p * success, r, i, usedSkills);

            // CP Season 3: once per game, +1 modifier to one agility test (roll of 1 always fails)
            var cpS3Success = _context.Season == Season.Season3
                && canUseSkill(CalculatorSkills.ConsummateProfessional, usedSkills)
                && action.Roll > 2
                ? 1m / 6 : 0m;
            if (cpS3Success > 0)
            {
                _calculator.Resolve(p * cpS3Success, r, i, usedSkills | CalculatorSkills.Pro);
            }
            failure -= cpS3Success;

            if (_proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                _calculator.Resolve(p * proSuccess * failure * success, r, i, usedSkills | CalculatorSkills.Pro);
                _calculator.Resolve(p * proSuccess * failureWithDivingTackle * success, r, i, usedSkills | CalculatorSkills.Pro | CalculatorSkills.DivingTackle);
                return;
            }

            _calculator.Resolve(p * lonerSuccess * failure * success, r - 1, i, usedSkills);
            _calculator.Resolve(p * lonerSuccess * failureWithDivingTackle * success, r - 1, i, usedSkills | CalculatorSkills.DivingTackle);
        }
    }
}
