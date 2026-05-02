using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Abstractions.Strategies.Blocking;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Blocking
{
    public class BlockStrategy(ICalculator calculator, IBlockSkillsHelper blockSkillsHelper, IBlockDice blockDice) : IActionStrategy
    {
        private Dictionary<Tuple<bool, int, CalculatorSkills>, decimal> _outcomes = new();
        private List<BlockResult> _successfulValues = new();
        private List<BlockResult> _nonCriticalFailureValues = new();
        private List<List<BlockResult>> _rolls = new();
        private int _rollsCount;
        private int _rerollCount;
        private bool _useBrawler;
        private bool _useHatred;
        private bool _usePro;
        private bool _useSavageBlow;
        private bool _canRerollOneDice;
        private bool _useLordOfChaos;
        private bool _rerollNonCriticalFailure;
        private decimal _proSuccess;
        private decimal _lonerSuccess;

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            foreach (var outcome in GetOutcomes(r, playerAction, usedSkills))
            {
                var ((outcomeNonCriticalFailure, rerollsRemaining, outcomeSkillsUsed), pOutcome) = outcome;

                calculator.Resolve(p * pOutcome, rerollsRemaining, i, usedSkills | outcomeSkillsUsed, outcomeNonCriticalFailure);
            }
        }

        private Dictionary<Tuple<bool, int, CalculatorSkills>, decimal> GetOutcomes(int r, PlayerAction playerAction, CalculatorSkills usedSkills)
        {
            var player = playerAction.Player;
            _proSuccess = player.ProSuccess;
            _lonerSuccess = player.LonerSuccess();

            var block = (Block) playerAction.Action;
            var numberOfSuccessfulResults = block.NumberOfSuccessfulResults;
            _successfulValues = blockDice.Rolls().Take(numberOfSuccessfulResults).ToList();
            _nonCriticalFailureValues = blockDice.Rolls().Skip(numberOfSuccessfulResults).Take(block.NumberOfNonCriticalFailures).ToList();
            _rerollNonCriticalFailure = block.RerollNonCriticalFailure;
            var successOnOneDie = _rerollNonCriticalFailure
                ? (decimal) _successfulValues.Count / 6
                : (decimal) (_successfulValues.Count + _nonCriticalFailureValues.Count) / 6;

            var numberOfDice = block.NumberOfDice;
            _rolls = blockDice.Rolls(numberOfDice);
            var success = 1 - (decimal)Math.Pow((double)(1 - successOnOneDie), numberOfDice);

            _rollsCount = _rolls.Count;
            var skillsToUse = blockSkillsHelper.SkillsToUse(player, block, r, usedSkills, successOnOneDie, success);
            _useBrawler = skillsToUse.Contains(CalculatorSkills.Brawler);
            _useHatred = skillsToUse.Contains(CalculatorSkills.Hatred);
            _usePro = skillsToUse.Contains(CalculatorSkills.Pro);
            _useSavageBlow = skillsToUse.Contains(CalculatorSkills.SavageBlow);
            _canRerollOneDice = skillsToUse.Contains(CalculatorSkills.UnstoppableMomentum)
                || skillsToUse.Contains(CalculatorSkills.WorkingInTandem);
            _useLordOfChaos = skillsToUse.Contains(CalculatorSkills.LordOfChaos);
            _outcomes = new Dictionary<Tuple<bool, int, CalculatorSkills>, decimal>();
            _rerollCount = 0;

            foreach (var roll in _rolls)
            {
                ProcessRoll(roll, r);
            }

            foreach (var reroll in _rolls)
            {
                AddOutcome(_rerollCount * _lonerSuccess / _rollsCount / _rollsCount, r - 1, CalculatorSkills.None, reroll);
            }

            return _outcomes;
        }

        private void ProcessRoll(List<BlockResult> roll, int r)
        {
            if (roll.Any(_successfulValues.Contains))
            {
                AddSuccess(1m / _rollsCount, r, CalculatorSkills.None);
                return;
            }

            var indexOfBothDown = roll.IndexOf(BlockResult.BothDown);

            if (roll.Any(_nonCriticalFailureValues.Contains) && !_rerollNonCriticalFailure)
            {
                if (_useBrawler && indexOfBothDown != -1)
                {
                    var rollWithoutBothDown = new List<BlockResult>(roll);
                    rollWithoutBothDown.RemoveAt(indexOfBothDown);

                    if (rollWithoutBothDown.Any(_nonCriticalFailureValues.Contains))
                    {
                        RerollDieAt(roll, r, indexOfBothDown);
                        return;
                    }
                }

                AddOutcome(1m / _rollsCount, r, CalculatorSkills.None, roll);
                return;
            }

            if (_useSavageBlow)
            {
                SavageBlow(r);
                return;
            }

            if (_useBrawler && indexOfBothDown != -1)
            {
                RerollDieAt(roll, r, indexOfBothDown);
                return;
            }

            var indexOfSkull = roll.IndexOf(BlockResult.Skull);
            if (_useHatred && indexOfSkull != -1)
            {
                RerollDieAt(roll, r, indexOfSkull);
                return;
            }

            if (_canRerollOneDice)
            {
                RerollOneDice(roll, r);
                return;
            }

            if (_useLordOfChaos)
            {
                LordOfChaos(roll, r);
                return;
            }

            if (_usePro)
            {
                Pro(roll, r);
                return;
            }

            if (r == 0)
            {
                AddOutcome(1m / _rollsCount, r, CalculatorSkills.None, roll);
                return;
            }

            AddOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, CalculatorSkills.None, roll);

            _rerollCount++;
        }

        private void RerollOneDice(List<BlockResult> roll, int r)
        {
            var indexOfLowestValue = roll.IndexOf(roll.Min());
            foreach (var face in blockDice.Rolls())
            {
                var umReroll = new List<BlockResult>(roll) { [indexOfLowestValue] = face };
                AddOutcome(1m / _rollsCount / 6, r, CalculatorSkills.None, umReroll);
            }
        }

        private void LordOfChaos(List<BlockResult> roll, int r)
        {
            var indexOfLowestValue = roll.IndexOf(roll.Min());
            foreach (var face in blockDice.Rolls())
            {
                var lcReroll = new List<BlockResult>(roll) { [indexOfLowestValue] = face };
                var p = 1m / _rollsCount / 6;

                if (lcReroll.Any(_successfulValues.Contains))
                {
                    AddSuccess(p, r, CalculatorSkills.LordOfChaos);
                    continue;
                }

                if (lcReroll.Any(_nonCriticalFailureValues.Contains))
                {
                    AddNonCriticalFailure(p, r, CalculatorSkills.LordOfChaos);
                    continue;
                }

                if (r == 0)
                {
                    continue;
                }

                foreach (var reroll in _rolls)
                {
                    AddOutcome(p * _lonerSuccess / _rollsCount, r - 1, CalculatorSkills.LordOfChaos, reroll);
                }
            }
        }

        private void Pro(List<BlockResult> roll, int r)
        {
            AddOutcome((1 - _proSuccess) / _rollsCount, r, CalculatorSkills.Pro, roll);

            var indexOfLowestValue = roll.IndexOf(roll.Min());
            foreach (var face in blockDice.Rolls())
            {
                var proReroll = new List<BlockResult>(roll) { [indexOfLowestValue] = face };
                AddOutcome(_proSuccess / _rollsCount / 6, r, CalculatorSkills.Pro, proReroll);
            }
        }

        private void SavageBlow(int r)
        {
            foreach (var reroll in _rolls)
            {
                AddOutcome(1m / _rollsCount / _rollsCount, r, CalculatorSkills.SavageBlow, reroll);
            }
        }

        private void RerollDieAt(IReadOnlyCollection<BlockResult> roll, int r, int rerollIndex)
        {
            foreach (var face in blockDice.Rolls())
            {
                if (_successfulValues.Contains(face))
                {
                    AddSuccess(1m / _rollsCount / 6, r, CalculatorSkills.None);
                    continue;
                }

                var rerolled = new List<BlockResult>(roll) { [rerollIndex] = face };

                if (!_usePro)
                {
                    AddOutcome(1m / _rollsCount / 6, r, CalculatorSkills.None, rerolled);
                    continue;
                }

                AddOutcome((1 - _proSuccess) / _rollsCount / 6, r, CalculatorSkills.Pro, rerolled);
                ProAfterReroll(rerolled, r, rerollIndex);
            }
        }

        private void ProAfterReroll(IReadOnlyList<BlockResult> roll, int r, int excludeIndex)
        {
            var indexOfLowestValue = -1;
            var lowestValue = (BlockResult)int.MaxValue;

            for (var i = 0; i < roll.Count; i++)
            {
                var rollValue = roll[i];

                if (i == excludeIndex || rollValue >= lowestValue)
                {
                    continue;
                }

                indexOfLowestValue = i;
                lowestValue = rollValue;
            }

            if (indexOfLowestValue == -1)
            {
                return;
            }

            foreach (var face in blockDice.Rolls())
            {
                var proReroll = new List<BlockResult>(roll) { [indexOfLowestValue] = face };
                AddOutcome(_proSuccess / _rollsCount / 36, r, CalculatorSkills.Pro, proReroll);
            }
        }

        private void AddOutcome(decimal p, int r, CalculatorSkills usedSkills, IReadOnlyCollection<BlockResult> roll)
        {
            if (p == 0)
            {
                return;
            }

            if (roll.Any(_successfulValues.Contains))
            {
                AddSuccess(p, r, usedSkills);
                return;
            }

            if (!roll.Any(_nonCriticalFailureValues.Contains))
            {
                return;
            }

            AddNonCriticalFailure(p, r, usedSkills);
        }

        private void AddSuccess(decimal p, int r, CalculatorSkills usedSkills) =>
            AddOrUpdateOutcomes(p, new Tuple<bool, int, CalculatorSkills>(false, r, usedSkills));

        private void AddNonCriticalFailure(decimal p, int r, CalculatorSkills usedSkills) =>
            AddOrUpdateOutcomes(p, new Tuple<bool, int, CalculatorSkills>(true, r, usedSkills));

        private void AddOrUpdateOutcomes(decimal p, Tuple<bool, int, CalculatorSkills> key)
        {
            if (_outcomes.ContainsKey(key))
            {
                _outcomes[key] += p;
            }
            else
            {
                _outcomes.Add(key, p);
            }
        }
    }
}
