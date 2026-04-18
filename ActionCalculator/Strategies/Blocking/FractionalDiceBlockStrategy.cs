using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Abstractions.Strategies.Blocking;

using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Blocking
{
    public class FractionalDiceBlockStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IBlockSkillsHelper _blockSkillsHelper;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;
        private Dictionary<Tuple<bool, int, CalculatorSkills>, decimal> _outcomes = new();
        private List<int> _successfulValues = new();
        private List<int> _nonCriticalFailureValues = new();
        private List<List<int>> _rolls = new();
        private int _rollsCount;
        private int _rerollCount;
        private bool _useBrawler;
        private bool _usePro;
        private bool _useBrawlerAndPro;
        private bool _useSavageBlow;
        private bool _rerollNonCriticalFailure;
        private decimal _proSuccess;
        private decimal _lonerSuccess;

        public FractionalDiceBlockStrategy(ICalculator calculator, IBlockSkillsHelper blockSkillsHelper, IProHelper proHelper, ID6 d6)
        {
            _calculator = calculator;
            _blockSkillsHelper = blockSkillsHelper;
            _proHelper = proHelper;
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

            var block = (Block)playerAction.Action;
            _proSuccess = player.ProSuccess;
            _lonerSuccess = player.LonerSuccess();

            var numberOfSuccessfulResults = block.NumberOfSuccessfulResults;
            _successfulValues = _d6.Rolls().Take(numberOfSuccessfulResults).ToList();
            _nonCriticalFailureValues = _d6.Rolls().Skip(numberOfSuccessfulResults).Take(block.NumberOfNonCriticalFailures).ToList();
            var successOnOneDie = _rerollNonCriticalFailure
                ? (decimal)_successfulValues.Count / 6
                : (decimal)(_successfulValues.Count + _nonCriticalFailureValues.Count) / 6;

            var numberOfDice = -block.NumberOfDice;
            _rolls = _d6.Rolls(numberOfDice);
            var success = (decimal) Math.Pow((double) successOnOneDie, numberOfDice);

            _rollsCount = _rolls.Count;
            var skillsToUse = _blockSkillsHelper.SkillsToUse(player, block, r, usedSkills, successOnOneDie, success);
            _useBrawler = skillsToUse.Contains(CalculatorSkills.Brawler);
            _usePro = skillsToUse.Contains(CalculatorSkills.Pro);
            _useBrawlerAndPro = _useBrawler && _proHelper.UsePro(player, block, r, usedSkills, successOnOneDie * successOnOneDie, success);
            _useSavageBlow = skillsToUse.Contains(CalculatorSkills.SavageBlow);
            _rerollNonCriticalFailure = block.RerollNonCriticalFailure;
            _outcomes = new Dictionary<Tuple<bool, int, CalculatorSkills>, decimal>();
            _rerollCount = 0;

            foreach (var roll in _rolls)
            {
                var failureCount = roll.Count - roll.Count(_successfulValues.Contains);
                var nonCriticalFailureCount = roll.Count(_nonCriticalFailureValues.Contains);
                var indexOfBothDown = roll.IndexOf(2);

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

        private void OneFailure(IReadOnlyCollection<int> roll, int r, int nonCriticalFailureCount, int indexOfBothDown)
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
            for (var i = 2; i <= 6; i++)
            {
                if (_successfulValues.Contains(i))
                {
                    AddSuccess(1m / _rollsCount / 6, r, CalculatorSkills.None);
                    continue;
                }

                if (_nonCriticalFailureValues.Contains(i))
                {
                    AddNonCriticalFailure(1m / _rollsCount / 6, r, CalculatorSkills.None);
                }
            }
        }

        private void Pro(IReadOnlyCollection<int> roll, int r)
        {
            AddOutcome((1 - _proSuccess) / _rollsCount, r, CalculatorSkills.Pro, roll);

            for (var i = 2; i <= 6; i++)
            {
                if (_successfulValues.Contains(i))
                {
                    AddSuccess(_proSuccess / _rollsCount / 6, r, CalculatorSkills.Pro);
                    continue;
                }

                if (_nonCriticalFailureValues.Contains(i))
                {
                    AddNonCriticalFailure(_proSuccess / _rollsCount / 6, r, CalculatorSkills.Pro);
                }
            }
        }

        private void TwoFailures(IReadOnlyList<int> roll, int r, int nonCriticalFailureCount, int indexOfBothDown)
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

            if (r == 0)
            {
                AddOutcome(1m / _rollsCount, r, CalculatorSkills.None, roll);
            }

            AddOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, CalculatorSkills.None, roll);

            _rerollCount++;
        }

        private void BrawlerAndPro(IReadOnlyList<int> roll, int r, int indexOfBothDown)
        {
            var brawlerRolls = new List<List<int>>();

            for (var i = 2; i <= 6; i++)
            {
                var rolledNonCriticalFailure = _nonCriticalFailureValues.Contains(i);
                if (!_successfulValues.Contains(i) && !rolledNonCriticalFailure)
                {
                    continue;
                }

                var brawlerRoll = new List<int>(roll) { [indexOfBothDown] = i };

                if (rolledNonCriticalFailure)
                {
                    AddOutcome((1 - _proSuccess) / _rollsCount / 6, r, CalculatorSkills.Pro, brawlerRoll);
                    continue;
                }

                brawlerRolls.Add(brawlerRoll);
            }

            var indexOfLowestValue = -1;
            var lowestValue = 999;

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

                for (var i = 2; i <= 6; i++)
                {
                    var proAndBrawlerRoll = new List<int>(brawlerRoll) { [indexOfLowestValue] = i };
                    AddOutcome(_proSuccess / _rollsCount / 36, r, CalculatorSkills.Pro, proAndBrawlerRoll);
                }
            }
        }

        private void ThreeFailures(IReadOnlyCollection<int> roll, int r, int nonCriticalFailureCount)
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

            AddOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, CalculatorSkills.None, roll);

            _rerollCount++;
        }
        
        private void SavageBlowOneFailure(IReadOnlyCollection<int> roll, int r)
        {
            var rollList = roll.ToList();
            var failureIndex = rollList.FindIndex(v => !_successfulValues.Contains(v));

            for (var i = 1; i <= 6; i++)
            {
                var reroll = new List<int>(rollList) { [failureIndex] = i };
                AddOutcome(1m / _rollsCount / 6, r, CalculatorSkills.SavageBlow, reroll);
            }
        }

        private void SavageBlowTwoFailures(IReadOnlyList<int> roll, int r)
        {
            var failureIndices = Enumerable.Range(0, roll.Count)
                .Where(i => !_successfulValues.Contains(roll[i]))
                .ToList();

            for (var i = 1; i <= 6; i++)
            {
                for (var j = 1; j <= 6; j++)
                {
                    var reroll = new List<int>(roll) { [failureIndices[0]] = i, [failureIndices[1]] = j };
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

        private void AddOutcome(decimal p, int r, CalculatorSkills usedSkills, IReadOnlyCollection<int> roll)
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
