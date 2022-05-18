using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.BallHandling
{
    public class PassStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public PassStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((lonerSuccess, proSuccess, canUseSkill), action, i) = playerAction;
            var (success, failure) = action;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            var inaccuratePass = action.NonCriticalFailureOnOneDie;
            var rerollInaccuratePass = action.RerollNonCriticalFailure;
            var accuratePassAfterFailure = (failure + (rerollInaccuratePass ? inaccuratePass : 0)) * success;
            var inaccuratePassAfterFailure = (failure + (rerollInaccuratePass ? inaccuratePass : 0)) * inaccuratePass;
            var inaccuratePassWithoutReroll = rerollInaccuratePass ? 0m : inaccuratePass;

            if (canUseSkill(Skills.Pass, usedSkills) && action.ActionType == ActionType.Pass || canUseSkill(Skills.TheBallista, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills, accuratePassAfterFailure, inaccuratePassWithoutReroll + inaccuratePassAfterFailure);
                return;
            }

            if (_proHelper.UsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(inaccuratePassWithoutReroll, r, i, usedSkills, true);

                usedSkills |= Skills.Pro;
                ExecuteReroll(p * proSuccess, r, i, usedSkills | Skills.Pro, accuratePassAfterFailure, inaccuratePassAfterFailure);
                return;
            }

            if (r > 0 && rerollInaccuratePass)
            {
                ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills | Skills.Pro, accuratePassAfterFailure, inaccuratePassAfterFailure);
                _actionMediator.Resolve(p * inaccuratePass * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                return;
            }

            _actionMediator.Resolve(p * inaccuratePass, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal accuratePass, decimal inaccuratePass)
        {
            _actionMediator.Resolve(p * accuratePass, r, i, usedSkills);
            _actionMediator.Resolve(p * inaccuratePass, r, i, usedSkills, true);
        }
    }
}