using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies
{
    public class HypnogazeStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public HypnogazeStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var hypnogaze = (Hypnogaze) playerAction.Action;
            var success = hypnogaze.Success;
            var failure = hypnogaze.Failure;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * hypnogaze.Success, r, i, usedSkills);

            p *= hypnogaze.Failure;

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
                _actionMediator.Resolve(p * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal success, decimal failure)
        {
            _actionMediator.Resolve(p * success, r, i, usedSkills);
            _actionMediator.Resolve(p * failure, r, i, usedSkills, true);
        }
    }
}