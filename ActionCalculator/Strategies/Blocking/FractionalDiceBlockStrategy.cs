using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Abstractions.Strategies.Blocking;

using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Blocking
{
    public class FractionalDiceBlockStrategy(ICalculator calculator, IBlockSkillsHelper blockSkillsHelper, IProHelper proHelper, IBlockDice blockDice) : IActionStrategy
    {
        private Dictionary<Tuple<bool, int, CalculatorSkills>, decimal> _outcomes = new();
        private List<BlockResult> _successfulValues = new();
        private List<BlockResult> _nonCriticalFailureValues = new();
        private List<List<BlockResult>> _rolls = new();
        private int _rollsCount;
        private int _rerollCount;
        private bool _useBrawler;
        private bool _usePro;
        private bool _useBrawlerAndPro;
        private bool _useSavageBlow;
        private bool _useHatred;
        private bool _useUnstoppableMomentum;
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

            var block = (Block)playerAction.Action;
            _proSuccess = player.ProSuccess;
            _lonerSuccess = player.LonerSuccess();

            var numberOfSuccessfulResults = block.NumberOfSuccessfulResults;
            _successfulValues = blockDice.Rolls().Take(numberOfSuccessfulResults).ToList();
            _nonCriticalFailureValues = blockDice.Rolls().Skip(numberOfSuccessfulResults).Take(block.NumberOfNonCriticalFailures).ToList();
            var successOnOneDie = _rerollNonCriticalFailure
                ? (decimal)_successfulValues.Count / 6
                : (decimal)(_successfulValues.Count + _nonCriticalFailureValues.Count) / 6;

            var numberOfDice = -block.NumberOfDice;
            _rolls = blockDice.Rolls(numberOfDice);
            var success = (decimal) Math.Pow((double) successOnOneDie, numberOfDice);

            _rollsCount = _rolls.Count;
            var skillsToUse = blockSkillsHelper.SkillsToUse(player, block, r, usedSkills, successOnOneDie, success);
            _useBrawler = skillsToUse.Contains(CalculatorSkills.Brawler);
            _usePro = skillsToUse.Contains(CalculatorSkills.Pro);
            _useBrawlerAndPro = _useBrawler && proHelper.UsePro(player, block, r, usedSkills, successOnOneDie * successOnOneDie, success);
            _useSavageBlow = skillsToUse.Contains(CalculatorSkills.SavageBlow);
            _useHatred = skillsToUse.Contains(CalculatorSkills.Hatred);
            _useUnstoppableMomentum = skillsToUse.Contains(CalculatorSkills.UnstoppableMomentum)
                || skillsToUse.Contains(CalculatorSkills.WorkingInTandem);
            _useLordOfChaos = skillsToUse.Contains(CalculatorSkills.LordOfChaos);
            _rerollNonCriticalFailure = block.RerollNonCriticalFailure;
            _outcomes = new Dictionary<Tuple<bool, int, CalculatorSkills>, decimal>();
            _rerollCount = 0;

            foreach (var roll in _rolls)
            {
                var failureCount = roll.Count - roll.Count(_successfulValues.Contains);
                var nonCriticalFailureCount = roll.Count(_nonCriticalFailureValues.Contains);
                var indexOfBothDown = roll.IndexOf(BlockResult.BothDown);

                switch (failureCount)
                {
                    case 0:
                        AddSuccess(1m / _rollsCount, r, CalculatorSkills.None);
                        continue;
                    case 1:
                        OneFailure(roll, r, nonCriticalFailureCount, indexOfBothDown);
                        continue;
                    case 2:
                        TwoFailures(roll, r, nonCriticalFailureCount, indexOfBothDown);
                        continue;
                    default:
                        ThreeFailures(roll, r, nonCriticalFailureCount);
                        continue;
                }
            }

            foreach (var reroll in _rolls)
            {
                AddOutcome(_rerollCount * _lonerSuccess / _rollsCount / _rollsCount, r - 1, CalculatorSkills.None, reroll);
            }

            return _outcomes;
        }

        private void OneFailure(IReadOnlyCollection<BlockResult> roll, int r, int nonCriticalFailureCount, int indexOfBothDown)
        {
            if (nonCriticalFailureCount == 1 && !_rerollNonCriticalFailure)
            {
                AddNonCriticalFailure(1m / _rollsCount, r, CalculatorSkills.None);
                return;
            }

            if (_useSavageBlow)
            {
                SavageBlowOneFailure(roll, r);
                return;
            }

            if (_useBrawler && indexOfBothDown != -1)
            {
                Brawler(r);
                return;
            }

            if ((_useHatred && roll.Contains(BlockResult.Skull)) || _useUnstoppableMomentum)
            {
                RerollMinDie(roll, r);
                return;
            }

            if (_useLordOfChaos)
            {
                LordOfChaosReroll(roll, r);
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

        private void Brawler(int r)
        {
            foreach (var face in blockDice.Rolls().Where(f => f != BlockResult.Skull))
            {
                if (_successfulValues.Contains(face))
                {
                    AddSuccess(1m / _rollsCount / 6, r, CalculatorSkills.None);
                    continue;
                }

                if (_nonCriticalFailureValues.Contains(face))
                {
                    AddNonCriticalFailure(1m / _rollsCount / 6, r, CalculatorSkills.None);
                }
            }
        }

        private void Pro(IReadOnlyCollection<BlockResult> roll, int r)
        {
            AddOutcome((1 - _proSuccess) / _rollsCount, r, CalculatorSkills.Pro, roll);

            foreach (var face in blockDice.Rolls().Where(f => f != BlockResult.Skull))
            {
                if (_successfulValues.Contains(face))
                {
                    AddSuccess(_proSuccess / _rollsCount / 6, r, CalculatorSkills.Pro);
                    continue;
                }

                if (_nonCriticalFailureValues.Contains(face))
                {
                    AddNonCriticalFailure(_proSuccess / _rollsCount / 6, r, CalculatorSkills.Pro);
                }
            }
        }

        private void TwoFailures(IReadOnlyList<BlockResult> roll, int r, int nonCriticalFailureCount, int indexOfBothDown)
        {
            if (nonCriticalFailureCount == 2 && !_rerollNonCriticalFailure)
            {
                AddNonCriticalFailure(1m / _rollsCount, r, CalculatorSkills.None);
                return;
            }

            if (_useSavageBlow)
            {
                SavageBlowTwoFailures(roll, r);
                return;
            }

            if (_useBrawlerAndPro && indexOfBothDown != -1)
            {
                BrawlerAndPro(roll, r, indexOfBothDown);
                return;
            }

            if ((_useHatred && roll.Contains(BlockResult.Skull)) || _useUnstoppableMomentum)
            {
                RerollMinDie(roll, r);
                return;
            }

            if (_useLordOfChaos)
            {
                LordOfChaosReroll(roll, r);
                return;
            }

            if (r == 0)
            {
                AddOutcome(1m / _rollsCount, r, CalculatorSkills.None, roll);
            }

            AddOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, CalculatorSkills.None, roll);

            _rerollCount++;
        }

        private void BrawlerAndPro(IReadOnlyList<BlockResult> roll, int r, int indexOfBothDown)
        {
            var brawlerRolls = new List<List<BlockResult>>();

            foreach (var face in blockDice.Rolls().Where(f => f != BlockResult.Skull))
            {
                var rolledNonCriticalFailure = _nonCriticalFailureValues.Contains(face);
                if (!_successfulValues.Contains(face) && !rolledNonCriticalFailure)
                {
                    continue;
                }

                var brawlerRoll = new List<BlockResult>(roll) { [indexOfBothDown] = face };

                if (rolledNonCriticalFailure)
                {
                    AddOutcome((1 - _proSuccess) / _rollsCount / 6, r, CalculatorSkills.Pro, brawlerRoll);
                    continue;
                }

                brawlerRolls.Add(brawlerRoll);
            }

            var indexOfLowestValue = -1;
            var lowestValue = (BlockResult)int.MaxValue;

            for (var i = 0; i < roll.Count; i++)
            {
                var rollValue = roll[i];

                if (i == indexOfBothDown || rollValue >= lowestValue)
                {
                    continue;
                }

                indexOfLowestValue = i;
                lowestValue = rollValue;
            }

            foreach (var brawlerRoll in brawlerRolls)
            {
                AddOutcome((1 - _proSuccess) / _rollsCount / 6, r, CalculatorSkills.Pro, brawlerRoll);

                foreach (var face in blockDice.Rolls().Where(f => f != BlockResult.Skull))
                {
                    var proAndBrawlerRoll = new List<BlockResult>(brawlerRoll) { [indexOfLowestValue] = face };
                    AddOutcome(_proSuccess / _rollsCount / 36, r, CalculatorSkills.Pro, proAndBrawlerRoll);
                }
            }
        }

        private void ThreeFailures(IReadOnlyCollection<BlockResult> roll, int r, int nonCriticalFailureCount)
        {
            if (nonCriticalFailureCount == 3 && !_rerollNonCriticalFailure)
            {
                AddNonCriticalFailure(1m / _rollsCount, r, CalculatorSkills.None);
                return;
            }

            if (_useSavageBlow)
            {
                SavageBlowThreeFailures(r);
                return;
            }

            if ((_useHatred && roll.Contains(BlockResult.Skull)) || _useUnstoppableMomentum)
            {
                RerollMinDie(roll, r);
                return;
            }

            if (_useLordOfChaos)
            {
                LordOfChaosReroll(roll, r);
                return;
            }

            AddOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, CalculatorSkills.None, roll);

            _rerollCount++;
        }

        private void RerollMinDie(IEnumerable<BlockResult> roll, int r)
        {
            var rollList = roll.ToList();
            var indexOfMin = rollList.IndexOf(rollList.Min());
            foreach (var face in blockDice.Rolls())
            {
                var reroll = new List<BlockResult>(rollList) { [indexOfMin] = face };
                AddOutcome(1m / _rollsCount / 6, r, CalculatorSkills.None, reroll);
            }
        }

        private void LordOfChaosReroll(IEnumerable<BlockResult> roll, int r)
        {
            var rollList = roll.ToList();
            var indexOfMin = rollList.IndexOf(rollList.Min());
            foreach (var face in blockDice.Rolls())
            {
                var lcReroll = new List<BlockResult>(rollList) { [indexOfMin] = face };
                var p = 1m / _rollsCount / 6;

                if (lcReroll.All(_successfulValues.Contains))
                {
                    AddSuccess(p, r, CalculatorSkills.LordOfChaos);
                    continue;
                }

                if (lcReroll.All(_successfulValues.Concat(_nonCriticalFailureValues).Contains))
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

        private void SavageBlowOneFailure(IReadOnlyCollection<BlockResult> roll, int r)
        {
            var rollList = roll.ToList();
            var failureIndex = rollList.FindIndex(v => !_successfulValues.Contains(v));

            foreach (var face in blockDice.Rolls())
            {
                var reroll = new List<BlockResult>(rollList) { [failureIndex] = face };
                AddOutcome(1m / _rollsCount / 6, r, CalculatorSkills.SavageBlow, reroll);
            }
        }

        private void SavageBlowTwoFailures(IReadOnlyList<BlockResult> roll, int r)
        {
            var failureIndices = Enumerable.Range(0, roll.Count)
                .Where(i => !_successfulValues.Contains(roll[i]))
                .ToList();

            foreach (var face1 in blockDice.Rolls())
            {
                foreach (var face2 in blockDice.Rolls())
                {
                    var reroll = new List<BlockResult>(roll) { [failureIndices[0]] = face1, [failureIndices[1]] = face2 };
                    AddOutcome(1m / _rollsCount / 36, r, CalculatorSkills.SavageBlow, reroll);
                }
            }
        }

        private void SavageBlowThreeFailures(int r)
        {
            foreach (var reroll in _rolls)
            {
                AddOutcome(1m / _rollsCount / _rollsCount, r, CalculatorSkills.SavageBlow, reroll);
            }
        }

        private void AddOutcome(decimal p, int r, CalculatorSkills usedSkills, IReadOnlyCollection<BlockResult> roll)
        {
            if (p == 0)
            {
                return;
            }

            if (roll.All(_successfulValues.Contains))
            {
                AddSuccess(p, r, usedSkills);
                return;
            }

            if (roll.All(_successfulValues.Concat(_nonCriticalFailureValues).Contains))
            {
                AddNonCriticalFailure(p, r, usedSkills);
            }
        }

        private void AddSuccess(decimal p, int r, CalculatorSkills usedSkills) => AddOrUpdateOutcomes(p, new Tuple<bool, int, CalculatorSkills>(false, r, usedSkills));

        private void AddNonCriticalFailure(decimal p, int r, CalculatorSkills usedSkills) => AddOrUpdateOutcomes(p, new Tuple<bool, int, CalculatorSkills>(true, r, usedSkills));

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
