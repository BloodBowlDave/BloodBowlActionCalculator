using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
    public class RerollableStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public RerollableStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var (lonerSuccess, proSuccess, _) = player;

            var success = _d6.Success(1, action.Roll);
            var failure = 1 - success;

            _calculator.Resolve(p * success, r, i, usedSkills);

            p *= failure * success;

            if (_proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                _calculator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _calculator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}