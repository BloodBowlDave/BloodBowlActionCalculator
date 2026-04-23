using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.BallHandling
{
	public class CatchInaccuratePassStrategy(ICalculator calculator, IProHelper proHelper, ID6 d6) : IActionStrategy
    {
        private const decimal ScatterToTarget = 24m / 512;
        private const decimal ScatterToTargetOrAdjacent = 240m / 512;
        private const decimal ScatterThenBounce = (ScatterToTargetOrAdjacent - ScatterToTarget) / 8;
        
        //for derivation of these values please see the BlastItTests class
        private const decimal BlastItScatterToTarget = 0.124267578125m;
        private const decimal BlastItScatterThenBounce = 0.0721588134765625m;
        private const decimal BlastItDivingCatchScatterToTargetOrAdjacent = 0.75610351562500m;

        private Dictionary<Tuple<int, CalculatorSkills>, decimal> _outcomes = new();

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            _outcomes = new Dictionary<Tuple<int, CalculatorSkills>, decimal>();

            var player = playerAction.Player;
            var action = playerAction.Action;
            var blastIt = usedSkills.Contains(CalculatorSkills.BlastIt);
            var canUseDivingCatch = player.CanUseSkill(CalculatorSkills.DivingCatch, usedSkills);

            var scatteredPassRoll = action.Roll + (blastIt ? 0 : 1) + (canUseDivingCatch ? 1 : 0);
            var success = d6.Success(1, scatteredPassRoll);
            var failure = 1 - success;

            CatchScatteredPass(r, playerAction, usedSkills, success, failure);

            var bouncingBallRoll = action.Roll + 1 + (canUseDivingCatch ? 1 : 0);
            success = d6.Success(1, bouncingBallRoll);
            failure = 1 - success;

            CatchBouncingBall(r, playerAction, usedSkills, success, failure);

            foreach (var ((rerolls, skills), value) in _outcomes)
            {
                calculator.Resolve(p * value, rerolls, i, skills);
            }
        }

        private void CatchScatteredPass(int r, PlayerAction playerAction, CalculatorSkills usedSkills, decimal success, decimal failure)
        {
            var canUseDivingCatch = playerAction.Player.CanUseSkill(CalculatorSkills.DivingCatch, usedSkills);
            var blastIt = usedSkills.Contains(CalculatorSkills.BlastIt);

            var scatter = canUseDivingCatch
                ? blastIt ? BlastItDivingCatchScatterToTargetOrAdjacent : ScatterToTargetOrAdjacent
                : blastIt ? BlastItScatterToTarget : ScatterToTarget;
            
            Catch(r, playerAction, usedSkills, scatter * success, scatter * failure * success);
        }

        private void CatchBouncingBall(int r, PlayerAction playerAction, CalculatorSkills usedSkills, decimal success, decimal failure)
        {
            if (playerAction.Player.CanUseSkill(CalculatorSkills.DivingCatch, usedSkills))
            {
                DivingCatch(r, playerAction, usedSkills, success, failure);
                return;
            }

            var scatter = usedSkills.Contains(CalculatorSkills.BlastIt)
                ? BlastItScatterThenBounce
                : ScatterThenBounce;
            
            Catch(r, playerAction, usedSkills, scatter * success, scatter * failure * success);
        }

        private void DivingCatch(int r, PlayerAction playerAction, CalculatorSkills usedSkills, decimal success, decimal failure)
        {
            var failDivingCatch = failure * failure;

            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var scatter = usedSkills.Contains(CalculatorSkills.BlastIt)
                ? BlastItScatterThenBounce
                : ScatterThenBounce;

            if (canUseSkill(CalculatorSkills.Catch, usedSkills))
            {
                AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r, usedSkills), failDivingCatch * scatter * (failure * success + success));
                return;
            }

            if (proHelper.UsePro(player, playerAction.Action, r, usedSkills, success, success))
            {
                var scatterSuccess = failDivingCatch * proSuccess * scatter * success;

                AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r, usedSkills | CalculatorSkills.Pro), scatterSuccess);
                AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r - 1, usedSkills | CalculatorSkills.Pro), scatterSuccess * failure * lonerSuccess);

                return;
            }

            if (r > 0)
            {
                var scatterSuccess = failDivingCatch * lonerSuccess * scatter * success;

                AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r - 1, usedSkills), scatterSuccess);
                AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r - 2, usedSkills), scatterSuccess * failure * lonerSuccess);

                return;
            }

            AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r, usedSkills), failure * scatter * success);
        }

        private void Catch(int r, PlayerAction playerAction, CalculatorSkills usedSkills, decimal successNoReroll, decimal successWithReroll)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var success = d6.Success(1, playerAction.Action.Roll);

            AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r, usedSkills), successNoReroll);

            if (canUseSkill(CalculatorSkills.Catch, usedSkills))
            {
                AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r, usedSkills), successWithReroll);
                return;
            }

            if (proHelper.UsePro(player, playerAction.Action, r, usedSkills, success, success))
            {
                AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r, usedSkills | CalculatorSkills.Pro), successWithReroll * proSuccess);
                return;
            }

            AddOrUpdateOutcomes(new Tuple<int, CalculatorSkills>(r - 1, usedSkills), successWithReroll * lonerSuccess);
        }

        private void AddOrUpdateOutcomes(Tuple<int, CalculatorSkills> outcome, decimal p)
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