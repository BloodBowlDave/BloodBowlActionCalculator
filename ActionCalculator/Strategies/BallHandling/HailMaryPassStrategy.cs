using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.BallHandling
{
    public class HailMaryPassStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public HailMaryPassStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, canUseSkill), (success, failure), i) = playerAction;

            if (canUseSkill(Skills.BlastIt, usedSkills))
            {
                usedSkills |= Skills.BlastIt;
            }

            _actionMediator.Resolve(p * success, r, i, usedSkills, true);

            if (_proHelper.UsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | Skills.Pro, true);
                return;
            }
            
            _actionMediator.Resolve(p * failure * rerollSuccess * success, r - 1, i, usedSkills, true);
        }
    }
}