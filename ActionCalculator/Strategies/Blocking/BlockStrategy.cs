using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Abstractions.Strategies.Blocking;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Blocking
{
    public class BlockStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IBlockSkillsHelper _blockSkillsHelper;
        private readonly ID6 _d6;
        private Dictionary<Tuple<bool, int, CalculatorSkills>, decimal> _outcomes = new();
        private List<int> _successfulValues = new();
        private List<int> _nonCriticalFailureValues = new();
        private List<List<int>> _rolls = new();
        private int _rollsCount;
        private int _rerollCount;
        private bool _useBrawler;
        private bool _canUseBrawler;
        private bool _useHatred;
        private bool _usePro;
        private bool _useSavageBlow;
        private bool _useUnstoppableMomentum;
        private bool _useLordOfChaos;
        private bool _rerollNonCriticalFailure;
        private decimal _proSuccess;
        private decimal _lonerSuccess;

        public BlockStrategy(ICalculator calculator, IBlockSkillsHelper blockSkillsHelper, ID6 d6)
        {
            _calculator = calculator;
            _blockSkillsHelper = blockSkillsHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            foreach (var outcome in GetOutcomes(r, playerAction, usedSkills))
            {
                var ((outcomeNonCriticalFailure, rerollsRemaining, outcomeSkillsUsed), pOutcome) = outcome;

                _calculator.Resolve(p * pOutcome, rerollsRemaining, i, usedSkills | outcomeSkillsUsed, outcomeNonCriticalFailure);
            }
        }

        private Dictionary<Tuple<bool, int, CalculatorSkills>, decimal> GetOutcomes(int r, PlayerAction playerAction, CalculatorSkills usedSkills)
        {
            var player = playerAction.Player;
            _proSuccess = player.ProSuccess;
            _lonerSuccess = player.LonerSuccess();

            var block = (Block) playerAction.Action;
            var numberOfSuccessfulResults = block.NumberOfSuccessfulResults;
            _successfulValues = _d6.Rolls().Take(numberOfSuccessfulResults).ToList();
            _nonCriticalFailureValues = _d6.Rolls().Skip(numberOfSuccessfulResults).Take(block.NumberOfNonCriticalFailures).ToList();
            _rerollNonCriticalFailure = block.RerollNonCriticalFailure;
            var successOnOneDie = _rerollNonCriticalFailure
                ? (decimal) _successfulValues.Count / 6
                : (decimal) (_successfulValues.Count + _nonCriticalFailureValues.Count) / 6;

            var numberOfDice = block.NumberOfDice;
            _rolls = _d6.Rolls(numberOfDice);
            var success = 1 - (decimal)Math.Pow((double)(1 - successOnOneDie), numberOfDice);

            _rollsCount = _rolls.Count;
            var skillsToUse = _blockSkillsHelper.SkillsToUse(player, block, r, usedSkills, successOnOneDie, success);
            _useBrawler = skillsToUse.Contains(CalculatorSkills.Brawler);
            _canUseBrawler = player.CanUseSkill(CalculatorSkills.Brawler, usedSkills);
            _useHatred = skillsToUse.Contains(CalculatorSkills.Hatred);
            _usePro = skillsToUse.Contains(CalculatorSkills.Pro);
            _useSavageBlow = skillsToUse.Contains(CalculatorSkills.SavageBlow);
            _useUnstoppableMomentum = skillsToUse.Contains(CalculatorSkills.UnstoppableMomentum);
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

        private void ProcessRoll(List<int> roll, int r)
        {
            if (roll.Any(_successfulValues.Contains))
            {
                AddSuccess(1m / _rollsCount, r, CalculatorSkills.None);
                return;
            }

            var indexOfBothDown = roll.IndexOf(2);

            if (roll.Any(_nonCriticalFailureValues.Contains) && !_rerollNonCriticalFailure)
            {
                if (_canUseBrawler && indexOfBothDown != -1)
                {
                    var rollWithoutBothDown = new List<int>(roll);
                    rollWithoutBothDown.RemoveAt(indexOfBothDown);

                    if (rollWithoutBothDown.Any(_nonCriticalFailureValues.Contains))
                    {
                        RerollDieAt(roll, r, indexOfBothDown);
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

            var indexOfSkull = roll.IndexOf(1);
            if (_useHatred && indexOfSkull != -1)
            {
                RerollDieAt(roll, r, indexOfSkull);
                return;
            }

            if (_useUnstoppableMomentum)
            {
                UnstoppableMomentum(roll, r);
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

        private void UnstoppableMomentum(List<int> roll, int r)
        {
            var indexOfLowestValue = roll.IndexOf(roll.Min());
            for (var i = 1; i <= 6; i++)
            {
                var umReroll = new List<int>(roll) { [indexOfLowestValue] = i };
                AddOutcome(1m / _rollsCount / 6, r, CalculatorSkills.None, umReroll);
            }
        }

        private void LordOfChaos(List<int> roll, int r)
        {
            var indexOfLowestValue = roll.IndexOf(roll.Min());
            for (var i = 1; i <= 6; i++)
            {
                var lcReroll = new List<int>(roll) { [indexOfLowestValue] = i };
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

        private void Pro(List<int> roll, int r)
        {
            AddOutcome((1 - _proSuccess) / _rollsCount, r, CalculatorSkills.Pro, roll);

            var indexOfLowestValue = roll.IndexOf(roll.Min());
            for (var i = 1; i <= 6; i++)
            {
                var proReroll = new List<int>(roll) { [indexOfLowestValue] = i };
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

        private void RerollDieAt(IReadOnlyCollection<int> roll, int r, int rerollIndex)
        {
            for (var i = 1; i <= 6; i++)
            {
                if (_successfulValues.Contains(i))
                {
                    AddSuccess(1m / _rollsCount / 6, r, CalculatorSkills.None);
                    continue;
                }

                var rerolled = new List<int>(roll) { [rerollIndex] = i };

                if (!_usePro)
                {
                    continue;
                }

                AddOutcome((1 - _proSuccess) / _rollsCount / 6, r, CalculatorSkills.Pro, rerolled);
                ProAfterReroll(rerolled, r, rerollIndex);
            }
        }

        private void ProAfterReroll(IReadOnlyList<int> roll, int r, int excludeIndex)
        {
            var indexOfLowestValue = -1;
            var lowestValue = 99;

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

            for (var i = 1; i <= 6; i++)
            {
                var proReroll = new List<int>(roll) { [indexOfLowestValue] = i };
                AddOutcome(_proSuccess / _rollsCount / 36, r, CalculatorSkills.Pro, proReroll);
            }
        }

        private void AddOutcome(decimal p, int r, CalculatorSkills usedSkills, IReadOnlyCollection<int> roll)
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
