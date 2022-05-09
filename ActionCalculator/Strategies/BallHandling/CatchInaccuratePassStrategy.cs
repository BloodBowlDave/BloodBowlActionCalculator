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
        private const decimal ScatterThenBounceToTarget = (ScatterToTargetOrAdjacent - ScatterToTarget) / 8;

        public CatchInaccuratePassStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (player, action, _) = playerAction;

            var roll = action.OriginalRoll + 1 + (player.CanUseSkill(Skills.DivingCatch, usedSkills) ? 1 : 0);

            var catchSuccess = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
            var catchFailure = 1 - catchSuccess;

            CatchScatteredPass(p, r, playerAction, usedSkills, catchSuccess, catchFailure);
            CatchBouncingBall(p, r, playerAction, usedSkills, catchSuccess, catchFailure);
        }

        private void CatchScatteredPass(decimal p, int r, PlayerAction playerAction, Skills usedSkills,
            decimal catchSuccess, decimal catchFailure)
        {
            var player = playerAction.Player;
            var successfulScatter = player.CanUseSkill(Skills.DivingCatch, usedSkills)
                ? ScatterToTargetOrAdjacent
                : ScatterToTarget;

            Catch(p, r, playerAction, usedSkills, successfulScatter * catchSuccess,
                successfulScatter * catchFailure * catchSuccess);
        }

        private void CatchBouncingBall(decimal p, int r, PlayerAction playerAction, Skills usedSkills,
            decimal catchSuccess, decimal catchFailure)
        {
            var player = playerAction.Player;

            if (player.CanUseSkill(Skills.DivingCatch, usedSkills))
            {
                DivingCatch(p, r, playerAction, usedSkills, catchSuccess, catchFailure);
                return;
            }

            Catch(p, r, playerAction, usedSkills, ScatterThenBounceToTarget * catchSuccess,
                ScatterThenBounceToTarget * catchFailure * catchSuccess);
        }

        private void DivingCatch(decimal p, int r, PlayerAction playerAction, Skills usedSkills, decimal catchSuccess, decimal catchFailure)
        {
            var failDivingCatch = catchFailure * catchFailure;

            var (player, _, i) = playerAction;

            if (player.CanUseSkill(Skills.Catch, usedSkills))
            {
                _actionMediator.Resolve(p * failDivingCatch * ScatterThenBounceToTarget * (catchFailure * catchSuccess + catchSuccess),
                    r, i, usedSkills);

                return;
            }

            if (_proHelper.CanUsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(
                    p * failDivingCatch * player.ProSuccess * ScatterThenBounceToTarget * catchSuccess, r, i,
                    usedSkills | Skills.Pro);

                if (r > 0)
                {
                    _actionMediator.Resolve(p * failDivingCatch * player.ProSuccess * ScatterThenBounceToTarget * catchFailure * player.RerollSuccess * catchSuccess,
                        r - 1, i, usedSkills | Skills.Pro);
                }

                return;
            }

            if (r > 0)
            {
                _actionMediator.Resolve(p * failDivingCatch * player.RerollSuccess * ScatterThenBounceToTarget * catchSuccess,
                    r - 1, i, usedSkills);

                if (r > 1)
                {
                    _actionMediator.Resolve(p * failDivingCatch * player.RerollSuccess * ScatterThenBounceToTarget
                                                     * catchFailure * player.RerollSuccess * catchSuccess, r - 2, i,
                        usedSkills);
                }

                return;
            }

            _actionMediator.Resolve(p * catchFailure * ScatterThenBounceToTarget * catchSuccess, r, i,
                usedSkills);
        }

        private void Catch(decimal p, int r, PlayerAction playerAction, Skills usedSkills,
            decimal successNoReroll, decimal successWithReroll)
        {
            var ((rerollSuccess, proSuccess, canUseSkill), _, i) = playerAction;

            _actionMediator.Resolve(p * successNoReroll, r, i, usedSkills);

            p *= successWithReroll;
            
            if (canUseSkill(Skills.Catch, usedSkills))
            {
                _actionMediator.Resolve(p, r, i, usedSkills);
                return;
            }

            if (_proHelper.CanUsePro(playerAction, r, usedSkills))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _actionMediator.Resolve(p * rerollSuccess, r - 1, i, usedSkills);
            }
        }
    }
}