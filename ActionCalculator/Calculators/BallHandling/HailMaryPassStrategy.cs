using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.BallHandling
{
    public class HailMaryPassStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProCalculator _proCalculator;

        public HailMaryPassStrategy(IActionMediator actionMediator, IProCalculator proCalculator)
        {
            _actionMediator = actionMediator;
            _proCalculator = proCalculator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, hasSkill), (success, failure), i) = playerAction;

            if (hasSkill(Skills.BlastIt, usedSkills))
            {
                usedSkills |= Skills.BlastIt;
            }

            _actionMediator.Resolve(p * success, r, i, usedSkills, true);

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | Skills.Pro, true);
                return;
            }

            if (r == 0)
            {
                return;
            }

            _actionMediator.Resolve(p * failure * rerollSuccess * success, r - 1, i, usedSkills, true);
        }
    }
}