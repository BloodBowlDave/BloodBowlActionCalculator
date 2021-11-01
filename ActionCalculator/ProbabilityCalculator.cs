using System;
using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
	public class ProbabilityCalculator : IProbabilityCalculator
    {
        private readonly ProbabilityComparer _probabilityComparer;

        private decimal[] _results;
        private int _maxRerolls;
        private Calculation _calculation;

        private const decimal ScatterToTarget = 24m / 512;
        private const decimal ScatterToTargetOrAdjacent = 240m / 512;
        private const decimal OneInSix = 1m / 6;

        public ProbabilityCalculator(ProbabilityComparer probabilityComparer)
        {
            _probabilityComparer = probabilityComparer;
        }

        private const int MaximumRerolls = 8;

        public IReadOnlyList<ProbabilityResult> Calculate(Calculation calculation)
        {
            _calculation = calculation;
            var probabilityResults = new ProbabilityResult[MaximumRerolls + 1];

            for (var rerolls = 0; rerolls < probabilityResults.Length; rerolls++)
			{
				_results = new decimal[calculation.PlayerActions.Length + 1];
                _maxRerolls = rerolls;

                Calculate(0, 1m, 0, -1, Skills.None);

                _results = _results.Where(x => x > 0).ToArray();

                if (rerolls > 0 && probabilityResults[rerolls - 1].Probabilities.SequenceEqual(_results, _probabilityComparer))
                {
                    break;
                }
                
                probabilityResults[rerolls] = new ProbabilityResult(_results.ToArray());
            }

            return probabilityResults;
        }

        private void Calculate(int r, decimal p, int i, int previousPlayerIndex, Skills usedSkills, bool inaccuratePass = false)
        {
            if (p <= 0)
            {
                return;
            }

            if (i == _calculation.PlayerActions.Length)
            {
                _results[r] += p;
                return;
            }

            var playerAction = _calculation.PlayerActions[i];
            var action = playerAction.Action;
            var player = playerAction.Player;

            if (previousPlayerIndex != player.Index)
            {
                usedSkills = Skills.None;
            }

            if (!inaccuratePass)
            {
                Calculate(r, p * action.Success, i + 1, player.Index, usedSkills);
            }

            var successAfterReroll = action.Failure * action.Success;

            switch (action.ActionType)
            {
                case ActionType.Other:
                    CalculateTeamReroll(r, p * successAfterReroll, i, player, usedSkills);
                    break;
                case ActionType.Dodge:
                    CalculateSkillReroll(r, p * successAfterReroll, i, player, usedSkills, Skills.Dodge);
                    break;
                case ActionType.Rush:
                    CalculateSkillReroll(r, p * successAfterReroll, i, player, usedSkills, Skills.SureFeet);
                    break;
                case ActionType.Catch:
                    CalculateCatch(r, p, i, usedSkills, inaccuratePass, action, player, successAfterReroll);
                    break;
                case ActionType.PickUp:
                    CalculateSkillReroll(r, p * successAfterReroll, i, player, usedSkills, Skills.SureHands);
                    break;
                case ActionType.Pass:
                    CalculatePass(r, p, i, usedSkills, action, player);
                    break;
                case ActionType.Block:
                    CalculateTeamReroll(r, p * successAfterReroll, i, player, usedSkills);
                    break;
                case ActionType.ThrowTeamMate:
                    break;
                case ActionType.Foul:
                case ActionType.ArmourBreak:
                case ActionType.Injury:
                case ActionType.OtherNonRerollable:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action.ActionType));
            }
        }

        private void CalculateCatch(int r, decimal p, int i, Skills usedSkills, bool inaccuratePass, Action action,
            Player player, decimal successAfterReroll)
        {
            if (inaccuratePass)
            {
                CalculateCatchFromInaccuratePass(r, p, i, usedSkills, action, player);
            }
            else
            {
                CalculateSkillReroll(r, p * successAfterReroll, i, player, usedSkills, Skills.Catch);
            }
        }

        private void CalculatePass(int r, decimal p, int i, Skills usedSkills, Action action, Player player)
        {
            var successAfterFailure = (action.Failure + action.NonCriticalFailure) * action.Success;
            var nonCriticalFailureAfterFailure = (action.Failure + action.NonCriticalFailure) * action.NonCriticalFailure;

            if (player.HasSkill(Skills.Pass) && !usedSkills.HasFlag(Skills.Pass) && action.RerollNonCriticalFailure)
            {
                Calculate(r, p * successAfterFailure, i + 1, player.Index, usedSkills | Skills.Pass);
                Calculate(r, p * nonCriticalFailureAfterFailure, i + 1, player.Index, usedSkills | Skills.Pass, true);
            }
            else if (r < _maxRerolls && action.RerollNonCriticalFailure)
            {
                CalculateTeamReroll(r, p * successAfterFailure, i, player, usedSkills);
                CalculateTeamReroll(r, p * nonCriticalFailureAfterFailure, i, player, usedSkills, true);
            }
            else
            {
                Calculate(r, p * action.NonCriticalFailure, i + 1, player.Index, usedSkills, true);
            }
        }

        private void CalculateCatchFromInaccuratePass(int r, decimal p, int actionIndex, Skills usedSkills, Action action, Player player)
        {
            var catchSuccess = action.Success > OneInSix ? action.Success - OneInSix : action.Success;

            var playerHasDivingCatch = player.HasSkill(Skills.DivingCatch);
            catchSuccess -= catchSuccess > OneInSix && playerHasDivingCatch ? OneInSix : 0;

            var successfulScatter = playerHasDivingCatch
                ? ScatterToTargetOrAdjacent
                : ScatterToTarget;

            var catchFailure = 1 - catchSuccess;

            Calculate(r, p * successfulScatter * catchSuccess, actionIndex + 1, player.Index, usedSkills);
            CalculateSkillReroll(r, p * successfulScatter * catchFailure * catchSuccess, actionIndex, player, usedSkills, Skills.Catch);
        }

        private void CalculateSkillReroll(int r, decimal p, int i, Player player, Skills usedSkills, Skills skill)
        {
            if (player.HasSkill(skill) && !usedSkills.HasFlag(skill))
            {
                Calculate(r, p, i + 1, player.Index, usedSkills | skill);
            }
            else
            {
                CalculateTeamReroll(r, p, i, player, usedSkills);
            }
        }

        private void CalculateTeamReroll(int r, decimal p, int i, Player player, Skills usedSkills, bool inaccuratePass = false)
        {
            if (r == _maxRerolls)
            {
                return;
            }

            var lonerSuccess = player.LonerSuccess != null ? (decimal) player.LonerSuccess : 1;
            Calculate(r + 1, p * lonerSuccess, i + 1, player.Index, usedSkills, inaccuratePass);
        }
    }
}
