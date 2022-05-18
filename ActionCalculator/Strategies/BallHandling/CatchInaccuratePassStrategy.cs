using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.BallHandling
{
    public class CatchInaccuratePassStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        private const decimal ScatterToTarget = 24m / 512;
        private const decimal ScatterToTargetOrAdjacent = 240m / 512;
        private const decimal ScatterThenBounce = (ScatterToTargetOrAdjacent - ScatterToTarget) / 8;

        public CatchInaccuratePassStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (player, action, _) = playerAction;

            var roll = action.OriginalRoll + 1 + (player.CanUseSkill(Skills.DivingCatch, usedSkills) ? 1 : 0);

            var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
            var failure = 1 - success;

            CatchScatteredPass(p, r, playerAction, usedSkills, success, failure);
            CatchBouncingBall(p, r, playerAction, usedSkills, success, failure);
        }

        private void CatchScatteredPass(decimal p, int r, PlayerAction playerAction, Skills usedSkills, decimal success, decimal failure)
        {
            var scatter = playerAction.Player.CanUseSkill(Skills.DivingCatch, usedSkills)
                ? ScatterToTargetOrAdjacent
                : ScatterToTarget;

            Catch(p, r, playerAction, usedSkills, scatter * success, scatter * failure * success);
        }

        private void CatchBouncingBall(decimal p, int r, PlayerAction playerAction, Skills usedSkills, decimal success, decimal failure)
        {
            if (playerAction.Player.CanUseSkill(Skills.DivingCatch, usedSkills))
            {
                DivingCatch(p, r, playerAction, usedSkills, success, failure);
                return;
            }

            Catch(p, r, playerAction, usedSkills, ScatterThenBounce * success, ScatterThenBounce * failure * success);
        }

        private void DivingCatch(decimal p, int r, PlayerAction playerAction, Skills usedSkills, decimal success, decimal failure)
        {
            var failDivingCatch = failure * failure;

            var ((lonerSuccess, proSuccess, canUseSkill), _, i) = playerAction;

            if (canUseSkill(Skills.Catch, usedSkills))
            {
                _actionMediator.Resolve(p * failDivingCatch * ScatterThenBounce * (failure * success + success), r, i, usedSkills);

                return;
            }

            if (_proHelper.UsePro(playerAction, r, usedSkills))
            {
                p *= failDivingCatch * proSuccess * ScatterThenBounce;
                usedSkills |= Skills.Pro;

                _actionMediator.Resolve(p * success, r, i, usedSkills);
                _actionMediator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills);

                return;
            }

            if (r > 0)
            {
                p *= failDivingCatch * lonerSuccess * ScatterThenBounce;

                _actionMediator.Resolve(p * success, r - 1, i, usedSkills);
                _actionMediator.Resolve(p * failure * lonerSuccess * success, r - 2, i, usedSkills);

                return;
            }

            _actionMediator.Resolve(p * failure * ScatterThenBounce * success, r, i, usedSkills);
        }

        private void Catch(decimal p, int r, PlayerAction playerAction, Skills usedSkills, decimal successNoReroll, decimal successWithReroll)
        {
            var ((lonerSuccess, proSuccess, canUseSkill), _, i) = playerAction;

            _actionMediator.Resolve(p * successNoReroll, r, i, usedSkills);

            p *= successWithReroll;
            
            if (canUseSkill(Skills.Catch, usedSkills))
            {
                _actionMediator.Resolve(p, r, i, usedSkills);
                return;
            }

            if (_proHelper.UsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
            
            _actionMediator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}