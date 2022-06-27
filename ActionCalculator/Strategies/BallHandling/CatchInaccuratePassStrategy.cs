using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.BallHandling
{
	public class CatchInaccuratePassStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;

        private const decimal ScatterToTarget = 24m / 512;
        private const decimal ScatterToTargetOrAdjacent = 240m / 512;
        private const decimal ScatterThenBounce = (ScatterToTargetOrAdjacent - ScatterToTarget) / 8;
        
        //for derivation of these values please see the BlastItTests class
        private const decimal BlastItScatterToTarget = 0.124267578125m;
        private const decimal BlastItScatterThenBounce = 0.0721588134765625m;
        private const decimal BlastItDivingCatchScatterToTargetOrAdjacent = 0.75610351562500m;

        private Dictionary<Tuple<int, Skills>, decimal> _outcomes = new();

        public CatchInaccuratePassStrategy(IActionMediator actionMediator, IProHelper proHelper, ID6 d6)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            _outcomes = new Dictionary<Tuple<int, Skills>, decimal>();
            
            var player = playerAction.Player;
            var action = playerAction.Action;
            var i = playerAction.Index;
            var blastIt = usedSkills.Contains(Skills.BlastIt);
            var canUseDivingCatch = player.CanUseSkill(Skills.DivingCatch, usedSkills);

            var scatteredPassRoll = action.Roll + (blastIt ? 0 : 1) + (canUseDivingCatch ? 1 : 0);
            var success = _d6.Success(1, scatteredPassRoll);
            var failure = 1 - success;

            CatchScatteredPass(r, playerAction, usedSkills, success, failure);

            var bouncingBallRoll = action.Roll + 1 + (canUseDivingCatch ? 1 : 0);
            success = _d6.Success(1, bouncingBallRoll);
            failure = 1 - success;

            CatchBouncingBall(r, playerAction, usedSkills, success, failure);

            foreach (var ((rerolls, skills), value) in _outcomes)
            {
                _actionMediator.Resolve(p * value, rerolls, i, skills);
            }
        }

        private void CatchScatteredPass(int r, PlayerAction playerAction, Skills usedSkills, decimal success, decimal failure)
        {
            var canUseDivingCatch = playerAction.Player.CanUseSkill(Skills.DivingCatch, usedSkills);
            var blastIt = usedSkills.Contains(Skills.BlastIt);

            var scatter = canUseDivingCatch
                ? blastIt ? BlastItDivingCatchScatterToTargetOrAdjacent : ScatterToTargetOrAdjacent
                : blastIt ? BlastItScatterToTarget : ScatterToTarget;
            
            Catch(r, playerAction, usedSkills, scatter * success, scatter * failure * success);
        }

        private void CatchBouncingBall(int r, PlayerAction playerAction, Skills usedSkills, decimal success, decimal failure)
        {
            if (playerAction.Player.CanUseSkill(Skills.DivingCatch, usedSkills))
            {
                DivingCatch(r, playerAction, usedSkills, success, failure);
                return;
            }

            var scatter = usedSkills.Contains(Skills.BlastIt)
                ? BlastItScatterThenBounce
                : ScatterThenBounce;
            
            Catch(r, playerAction, usedSkills, scatter * success, scatter * failure * success);
        }

        private void DivingCatch(int r, PlayerAction playerAction, Skills usedSkills, decimal success, decimal failure)
        {
            var failDivingCatch = failure * failure;

            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var scatter = usedSkills.Contains(Skills.BlastIt)
                ? BlastItScatterThenBounce
                : ScatterThenBounce;

            if (canUseSkill(Skills.Catch, usedSkills))
            {
                AddOrUpdateOutcomes(new Tuple<int, Skills>(r, usedSkills), failDivingCatch * scatter * (failure * success + success));
                return;
            }

            if (_proHelper.UsePro(player, playerAction.Action, r, usedSkills, success, success))
            {
                var scatterSuccess = failDivingCatch * proSuccess * scatter * success;
                
                AddOrUpdateOutcomes(new Tuple<int, Skills>(r, usedSkills | Skills.Pro), scatterSuccess);
                AddOrUpdateOutcomes(new Tuple<int, Skills>(r - 1, usedSkills | Skills.Pro), scatterSuccess * failure * lonerSuccess);
                
                return;
            }

            if (r > 0)
            {
                var scatterSuccess = failDivingCatch * lonerSuccess * scatter * success;

                AddOrUpdateOutcomes(new Tuple<int, Skills>(r - 1, usedSkills), scatterSuccess);
                AddOrUpdateOutcomes(new Tuple<int, Skills>(r - 2, usedSkills), scatterSuccess * failure * lonerSuccess);
                
                return;
            }

            AddOrUpdateOutcomes(new Tuple<int, Skills>(r, usedSkills), failure * scatter * success);
        }

        private void Catch(int r, PlayerAction playerAction, Skills usedSkills, decimal successNoReroll, decimal successWithReroll)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var success = playerAction.Action.Success;
            
            AddOrUpdateOutcomes(new Tuple<int, Skills>(r, usedSkills), successNoReroll);
            
            if (canUseSkill(Skills.Catch, usedSkills))
            {
                AddOrUpdateOutcomes(new Tuple<int, Skills>(r, usedSkills), successWithReroll);
                return;
            }

            if (_proHelper.UsePro(player, playerAction.Action, r, usedSkills, success, success))
            {
                AddOrUpdateOutcomes(new Tuple<int, Skills>(r, usedSkills | Skills.Pro), successWithReroll * proSuccess);
                return;
            }

            AddOrUpdateOutcomes(new Tuple<int, Skills>(r - 1, usedSkills), successWithReroll * lonerSuccess);
        }

        private void AddOrUpdateOutcomes(Tuple<int, Skills> outcome, decimal p)
        {
            if (_outcomes.ContainsKey(outcome))
            {
                _outcomes[outcome] += p;
            }
            else
            {
                _outcomes.Add(outcome, p);
            }
        }
    }
}