using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.BallHandling
{
    public class PassStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProCalculator _proCalculator;

        public PassStrategy(IActionMediator actionMediator, IProCalculator proCalculator)
        {
            _actionMediator = actionMediator;
            _proCalculator = proCalculator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, hasSkill), action, i) = playerAction;
            var (success, failure) = action;


            _actionMediator.Resolve(p * success, r, i, usedSkills);

            var inaccuratePass = action.NonCriticalFailure;
            var rerollInaccuratePass = action.RerollNonCriticalFailure;
            var accuratePassAfterFailure = p * (failure + (rerollInaccuratePass ? inaccuratePass : 0)) * success;
            var inaccuratePassAfterFailure = p * (failure + (rerollInaccuratePass ? inaccuratePass : 0)) * inaccuratePass;
            var inaccuratePassWithoutReroll = p * (rerollInaccuratePass ? 0m : inaccuratePass);

            if (hasSkill(Skills.Pass, usedSkills) && action.ActionType == ActionType.Pass || hasSkill(Skills.TheBallista, usedSkills))
            {
                _actionMediator.Resolve(accuratePassAfterFailure, r, i, usedSkills);
                _actionMediator.Resolve(inaccuratePassWithoutReroll + inaccuratePassAfterFailure, r, i, usedSkills, true);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(inaccuratePassWithoutReroll, r, i, usedSkills, true);

                usedSkills |= Skills.Pro;
                _actionMediator.Resolve(proSuccess * accuratePassAfterFailure, r, i, usedSkills);
                _actionMediator.Resolve(proSuccess * inaccuratePassAfterFailure, r, i, usedSkills, true);
                return;
            }

            if (r > 0 && rerollInaccuratePass)
            {
                _actionMediator.Resolve(rerollSuccess * accuratePassAfterFailure, r - 1, i, usedSkills);
                _actionMediator.Resolve(rerollSuccess * inaccuratePassAfterFailure, r - 1, i, usedSkills, true);
                return;
            }

            _actionMediator.Resolve(p * inaccuratePass, r, i, usedSkills, true);
        }
    }
}