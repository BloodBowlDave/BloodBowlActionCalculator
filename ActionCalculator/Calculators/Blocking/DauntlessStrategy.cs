using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Blocking
{
    public class DauntlessStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProCalculator _proCalculator;

        public DauntlessStrategy(IActionMediator actionMediator, IProCalculator proCalculator)
        {
            _actionMediator = actionMediator;
            _proCalculator = proCalculator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, hasSkill), action, i) = playerAction;
            var (success, failure) = action;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            p *= failure;

            if (action.RerollNonCriticalFailure)
            {
                if (hasSkill(Skills.BlindRage, usedSkills))
                {
                    _actionMediator.Resolve(p * success, r, i, usedSkills);
                    _actionMediator.Resolve(p * failure, r, i, usedSkills, true);
                    return;
                }

                if (_proCalculator.UsePro(playerAction, r, usedSkills))
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

            if (action.UsePro && _proCalculator.UsePro(playerAction, r, usedSkills))
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