using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Models;
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
        private readonly Dictionary<int, decimal> _blastItScatterToTargetLookup = new()
        {
            { 7, 0.1229248046875m },
            { 6, 0.124267578125m },
            { 5, 0.124267578125m },
            { 4, 0.124267578125m },
            { 3, 0.124267578125m },
            { 2, 0.1229248046875m }
        };
        private readonly Dictionary<int, decimal> _blastItScatterThenBounceLookup = new()
        {
            { 7, 0.07376861572265625m },
            { 6, 0.0721588134765625m },
            { 5, 0.0721588134765625m },
            { 4, 0.0721588134765625m },
            { 3, 0.0721588134765625m },
            { 2, 0.07376861572265625m }
        };
        private const decimal BlastItDivingCatchScatterToTargetOrAdjacent = 0.75610351562500m;

        public CatchInaccuratePassStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;
            var blastIt = usedSkills.Contains(Skills.BlastIt);
            var canUseDivingCatch = player.CanUseSkill(Skills.DivingCatch, usedSkills);

            var scatteredPassRoll = action.Roll + (blastIt ? 0 : 1) + (canUseDivingCatch ? 1 : 0);

            CatchScatteredPass(p, r, playerAction, usedSkills, scatteredPassRoll);

            var bouncingBallRoll = action.Roll + 1 + (canUseDivingCatch ? 1 : 0);

            CatchBouncingBall(p, r, playerAction, usedSkills, bouncingBallRoll);
        }

        private void CatchScatteredPass(decimal p, int r, PlayerAction playerAction, Skills usedSkills, int roll)
        {
            var canUseDivingCatch = playerAction.Player.CanUseSkill(Skills.DivingCatch, usedSkills);
            var blastIt = usedSkills.Contains(Skills.BlastIt);

            var scatter = canUseDivingCatch
                ? /*blastIt ? BlastItDivingCatchScatterToTargetOrAdjacent :*/ ScatterToTargetOrAdjacent
                : /*blastIt ? _blastItScatterToTargetLookup[roll.ThisOrMaximum(7)] :*/ ScatterToTarget;

            var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
            var failure = 1 - success;

            Catch(p, r, playerAction, usedSkills, scatter * success, scatter * failure * success);
        }

        private void CatchBouncingBall(decimal p, int r, PlayerAction playerAction, Skills usedSkills, int roll)
        {
            if (playerAction.Player.CanUseSkill(Skills.DivingCatch, usedSkills))
            {
                DivingCatch(p, r, playerAction, usedSkills, roll);
                return;
            }

            //var scatter = usedSkills.Contains(Skills.BlastIt)
            //    ? _blastItScatterThenBounceLookup[roll.ThisOrMinimum(2).ThisOrMaximum(7)]
            //    : ScatterThenBounce;

            var scatter = ScatterThenBounce;

            var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
            var failure = 1 - success;

            Catch(p, r, playerAction, usedSkills, scatter * success, scatter * failure * success);
        }

        private void DivingCatch(decimal p, int r, PlayerAction playerAction, Skills usedSkills, int roll)
        {
            var success = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
            var failure = 1 - success;

            var failDivingCatch = failure * failure;

            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var i = playerAction.Index;
            //var scatter = usedSkills.Contains(Skills.BlastIt) 
            //    ? _blastItScatterThenBounceLookup[roll.ThisOrMaximum(7)]
            //    : ScatterThenBounce;

            var scatter = ScatterThenBounce;

            if (canUseSkill(Skills.Catch, usedSkills))
            {
                _actionMediator.Resolve(p * failDivingCatch * scatter * (failure * success + success), r, i, usedSkills);
                return;
            }

            if (_proHelper.UsePro(player, playerAction.Action, r, usedSkills, success, success))
            {
                p *= failDivingCatch * proSuccess * scatter;
                usedSkills |= Skills.Pro;

                _actionMediator.Resolve(p * success, r, i, usedSkills);
                _actionMediator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills);

                return;
            }

            if (r > 0)
            {
                p *= failDivingCatch * lonerSuccess * scatter;

                _actionMediator.Resolve(p * success, r - 1, i, usedSkills);
                _actionMediator.Resolve(p * failure * lonerSuccess * success, r - 2, i, usedSkills);

                return;
            }

            _actionMediator.Resolve(p * failure * scatter * success, r, i, usedSkills);
        }

        private void Catch(decimal p, int r, PlayerAction playerAction, Skills usedSkills, decimal successNoReroll, decimal successWithReroll)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var success = playerAction.Action.Success;
            var i = playerAction.Index;

            _actionMediator.Resolve(p * successNoReroll, r, i, usedSkills);

            p *= successWithReroll;
            
            if (canUseSkill(Skills.Catch, usedSkills))
            {
                _actionMediator.Resolve(p, r, i, usedSkills);
                return;
            }

            if (_proHelper.UsePro(player, playerAction.Action, r, usedSkills, success, success))
            {
                _actionMediator.Resolve(p * proSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }
            
            _actionMediator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}