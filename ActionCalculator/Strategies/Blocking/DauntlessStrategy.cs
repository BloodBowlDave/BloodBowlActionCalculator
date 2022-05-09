using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.Blocking
{
    public class DauntlessStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public DauntlessStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, canUseSkill), action, i) = playerAction;
            var (success, failure) = action;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure;

            if (action.RerollNonCriticalFailure)
            {
                if (canUseSkill(Skills.BlindRage, usedSkills))
                {
                    _actionMediator.Resolve(p * success, r, i, usedSkills);
                    _actionMediator.Resolve(p * failure, r, i, usedSkills, true);
                    return;
                }

                if (_proHelper.UsePro(playerAction, r, usedSkills))
                {
                    usedSkills |= Skills.Pro;
                    _actionMediator.Resolve(p * proSuccess * success, r, i, usedSkills);
                    _actionMediator.Resolve(p * (proSuccess * failure + (1 - proSuccess)), r, i, usedSkills, true);
                    return;
                }

                if (r > 0)
                {
                    p *= rerollSuccess;
                    _actionMediator.Resolve(p * success, r - 1, i, usedSkills);
                    _actionMediator.Resolve(p * failure, r - 1, i, usedSkills, true);

                    return;
                }
            }

            if (action.UsePro && _proHelper.UsePro(playerAction, r, usedSkills))
            {
                usedSkills |= Skills.Pro;
                _actionMediator.Resolve(p * proSuccess * success, r, i, usedSkills);
                _actionMediator.Resolve(p * (proSuccess * failure + (1 - proSuccess)), r, i, usedSkills, true);

                return;
            }

            _actionMediator.Resolve(p, r, i, usedSkills, true);
        }
    }
}