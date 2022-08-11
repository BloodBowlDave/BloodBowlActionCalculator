using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies
{
    public class HypnogazeStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        public HypnogazeStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6)
        {
            _calculator = calculator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var hypnogaze = (Hypnogaze) playerAction.Action;
            var success = _d6.Success(1, hypnogaze.Roll);
            var failure = 1 - success;

            _calculator.Resolve(p * success, r, i, usedSkills);

            p *= failure;

            if (canUseSkill(Skills.MesmerisingDance, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills, success, failure);
            }

            if (_proHelper.UsePro(player, hypnogaze, r, usedSkills, success, success))
            {
                ExecuteReroll(p * proSuccess, r, i, usedSkills | Skills.Pro, success, failure);
                return;
            }

            if (r > 0 && hypnogaze.RerollFailure)
            {
                ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills, success, failure);
                _calculator.Resolve(p * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                return;
            }

            _calculator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal success, decimal failure)
        {
            _calculator.Resolve(p * success, r, i, usedSkills);
            _calculator.Resolve(p * failure, r, i, usedSkills, true);
        }
    }
}