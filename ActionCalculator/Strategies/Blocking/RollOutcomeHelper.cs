using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Strategies.Blocking
{
    public class RollOutcomeHelper : IRollOutcomeHelper
    {
        private readonly IBrawlerHelper _brawlerHelper;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;
        private Dictionary<Tuple<bool, int, Skills>, decimal> _outcomes = new();
        private List<int> _successfulValues = new();
        private List<int> _nonCriticalFailureValues = new();
        private int _rollsCount;
        private int _rerollCount;
        private bool _useBrawler;
        private bool _usePro;
        private bool _useBrawlerAndPro;
        private bool _rerollNonCriticalFailure;
        private decimal _proSuccess;
        private decimal _lonerSuccess;

        public RollOutcomeHelper(IBrawlerHelper brawlerHelper, IProHelper proHelper, ID6 d6)
        {
            _brawlerHelper = brawlerHelper;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public Dictionary<Tuple<bool, int, Skills>, decimal> GetRollOutcomes(int r, PlayerAction playerAction, Skills usedSkills)
        {
            var (player, action, _) = playerAction;
            _proSuccess = player.ProSuccess;
            _lonerSuccess = player.LonerSuccess;

            var numberOfSuccessfulResults = action.NumberOfSuccessfulResults;
            _successfulValues = _d6.Rolls().Take(numberOfSuccessfulResults).ToList();
            _nonCriticalFailureValues = _d6.Rolls().Skip(numberOfSuccessfulResults).Take(action.NumberOfNonCriticalFailures).ToList();

            var numberOfDice = action.NumberOfDice;
            var rolls = _d6.Rolls(numberOfDice);

            _rollsCount = rolls.Count;
            _useBrawler = _brawlerHelper.UseBrawler(r, playerAction, usedSkills);
            _usePro = _proHelper.UsePro(playerAction, r, usedSkills, action.SuccessOnOneDie, action.Success);
            _rerollNonCriticalFailure = action.RerollNonCriticalFailure;
            _outcomes = new Dictionary<Tuple<bool, int, Skills>, decimal>();
            _rerollCount = 0;
            
            foreach (var roll in rolls)
            {
                ProcessRoll(roll, r);
            }

            foreach (var reroll in rolls)
            {
                AddOutcome(_rerollCount * _lonerSuccess / _rollsCount / _rollsCount, r - 1, Skills.None, reroll);
            }

            return _outcomes;
        }

        private void ProcessRoll(List<int> roll, int r)
        {
            if (roll.Any(_successfulValues.Contains))
            {
                AddSuccess(1m / _rollsCount, r, Skills.None);
                return;
            }

            var indexOfBothDown = roll.IndexOf(2);

            if (_useBrawler && indexOfBothDown != -1)
            {
                Brawler(roll, r, indexOfBothDown);
                return;
            }

            if (_usePro)
            {
                Pro(roll, r);
                return;
            }

            if (r == 0 || roll.Any(_nonCriticalFailureValues.Contains) && !_rerollNonCriticalFailure)
            {
                AddOutcome(1m / _rollsCount, r, Skills.None, roll);
                return;
            }

            AddOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, Skills.None, roll);

            _rerollCount++;
        }

        private void Pro(List<int> roll, int r)
        {
            AddOutcome((1 - _proSuccess) / _rollsCount, r, Skills.Pro, roll);

            var indexOfLowestValue = roll.IndexOf(roll.Min());
            for (var i = 1; i <= 6; i++)
            {
                var proReroll = new List<int>(roll) {[indexOfLowestValue] = i};
                AddOutcome(_proSuccess / _rollsCount / 6, r, Skills.Pro, proReroll);
            }
        }

        private void Brawler(IReadOnlyCollection<int> roll, int r, int indexOfBothDown)
        {
            for (var i = 1; i <= 6; i++)
            {
                if (_successfulValues.Contains(i))
                {
                    AddSuccess(1m / _rollsCount / 6, r, Skills.None);
                    continue;
                }

                var brawlerReroll = new List<int>(roll) {[indexOfBothDown] = i};
                AddOutcome((1 - _proSuccess) / _rollsCount / 6, r, Skills.Pro, brawlerReroll);

                if (_usePro)
                {
                    ProAfterBrawler(brawlerReroll, r, indexOfBothDown);
                }
            }
        }

        private void ProAfterBrawler(IReadOnlyList<int> brawlerReroll, int r, int indexOfBothDown)
        {
            var indexOfLowestValue = -1;
            var lowestValue = 99;

            for (var i = 0; i < brawlerReroll.Count; i++)
            {
                var rollValue = brawlerReroll[i];

                if (i == indexOfBothDown || rollValue >= lowestValue)
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
                var proReroll = new List<int>(brawlerReroll) {[indexOfLowestValue] = i};
                AddOutcome(_proSuccess / _rollsCount / 36, r, Skills.Pro, proReroll);
            }
        }
        
        public Dictionary<Tuple<bool, int, Skills>, decimal> GetRollOutcomesForRedDice(int r, PlayerAction playerAction, Skills usedSkills)
        {
            var (player, action, _) = playerAction;
            _proSuccess = player.ProSuccess;
            _lonerSuccess = player.LonerSuccess;

            var numberOfSuccessfulResults = action.NumberOfSuccessfulResults;
            _successfulValues = _d6.Rolls().Take(numberOfSuccessfulResults).ToList();
            _nonCriticalFailureValues = _d6.Rolls().Skip(numberOfSuccessfulResults).Take(action.NumberOfNonCriticalFailures).ToList();

            var numberOfDice = -action.NumberOfDice;
            var rolls = _d6.Rolls(numberOfDice);

            _rollsCount = rolls.Count;
            _useBrawler = _brawlerHelper.UseBrawler(r, playerAction, usedSkills);
            _usePro = _proHelper.UsePro(playerAction, r, usedSkills, action.SuccessOnOneDie, action.Success);
            _useBrawlerAndPro = _brawlerHelper.UseBrawlerAndPro(r, playerAction, usedSkills);
            _rerollNonCriticalFailure = action.RerollNonCriticalFailure;
            _outcomes = new Dictionary<Tuple<bool, int, Skills>, decimal>();
            _rerollCount = 0;

            foreach (var roll in rolls)
            {
                var failureCount = roll.Count - roll.Count(_successfulValues.Contains);
                var nonCriticalFailureCount = roll.Count(_nonCriticalFailureValues.Contains);
                var indexOfBothDown = roll.IndexOf(2);

                switch (failureCount)
                {
                    case 0:
                        AddSuccess(1m / _rollsCount, r, Skills.None);
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

            foreach (var reroll in rolls)
            {
                AddRedDiceOutcome(_rerollCount * _lonerSuccess / _rollsCount / _rollsCount, r - 1, Skills.None, reroll);
            }

            return _outcomes;
        }

        private void ThreeFailures(IReadOnlyCollection<int> roll, int r, int nonCriticalFailureCount)
        {
            if (nonCriticalFailureCount == 3 && !_rerollNonCriticalFailure)
            {
                AddNonCriticalFailure(1m / _rollsCount, r, Skills.None);
                return;
            }

            AddRedDiceOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, Skills.None, roll);

            _rerollCount++;
        }

        private void TwoFailures(IReadOnlyList<int> roll, int r, int nonCriticalFailureCount, int indexOfBothDown)
        {
            if (nonCriticalFailureCount == 2 && !_rerollNonCriticalFailure)
            {
                AddNonCriticalFailure(1m / _rollsCount, r, Skills.None);
                return;
            }

            if (_useBrawlerAndPro && indexOfBothDown != -1)
            {
                BrawlerAndPro(roll, r, indexOfBothDown);
                return;
            }

            if (r == 0)
            {
                AddRedDiceOutcome(1m / _rollsCount, r, Skills.None, roll);
            }

            AddRedDiceOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, Skills.None, roll);

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

                var brawlerRoll = new List<int>(roll) {[indexOfBothDown] = i};

                if (rolledNonCriticalFailure)
                {
                    AddRedDiceOutcome((1 - _proSuccess) / _rollsCount / 6, r, Skills.Pro, brawlerRoll);
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
                AddRedDiceOutcome((1 - _proSuccess) / _rollsCount / 6, r, Skills.Pro, brawlerRoll);

                for (var i = 2; i <= 6; i++)
                {
                    var proAndBrawlerRoll = new List<int>(brawlerRoll) {[indexOfLowestValue] = i};
                    AddRedDiceOutcome(_proSuccess / _rollsCount / 36, r, Skills.Pro, proAndBrawlerRoll);
                }
            }
        }

        private void OneFailure(IReadOnlyCollection<int> roll, int r, int nonCriticalFailureCount, int indexOfBothDown)
        {
            if (nonCriticalFailureCount == 1 && !_rerollNonCriticalFailure)
            {
                AddNonCriticalFailure(1m / _rollsCount / 6, r, Skills.None);
                return;
            }

            if (_useBrawler && indexOfBothDown != -1)
            {
                Brawler(r);
                return;
            }

            if (_usePro)
            {
                RedDicePro(roll, r);
                return;
            }

            if (r == 0)
            {
                AddRedDiceOutcome(1m / _rollsCount, r, Skills.None, roll);
                return;
            }

            AddRedDiceOutcome((1 - _lonerSuccess) / _rollsCount, r - 1, Skills.None, roll);

            _rerollCount++;
        }

        private void RedDicePro(IReadOnlyCollection<int> roll, int r)
        {
            AddRedDiceOutcome((1 - _proSuccess) / _rollsCount, r, Skills.Pro, roll);

            for (var i = 2; i <= 6; i++)
            {
                if (_successfulValues.Contains(i))
                {
                    AddSuccess(_proSuccess / _rollsCount / 6, r, Skills.Pro);
                    continue;
                }

                if (_nonCriticalFailureValues.Contains(i))
                {
                    AddNonCriticalFailure(_proSuccess / _rollsCount / 6, r, Skills.Pro);
                }
            }
        }

        private void Brawler(int r)
        {
            for (var i = 2; i <= 6; i++)
            {
                if (_successfulValues.Contains(i))
                {
                    AddSuccess(1m / _rollsCount / 6, r, Skills.None);
                    continue;
                }

                if (_nonCriticalFailureValues.Contains(i))
                {
                    AddNonCriticalFailure(1m / _rollsCount / 6, r, Skills.None);
                }
            }
        }

        private void AddOutcome(decimal p, int r, Skills usedSkills, IReadOnlyCollection<int> roll)
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

        private void AddRedDiceOutcome(decimal p, int r, Skills usedSkills, IReadOnlyCollection<int> roll)
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

        private void AddSuccess(decimal p, int r, Skills usedSkills) => AddOrUpdateOutcomes(p, new Tuple<bool, int, Skills>(false, r, usedSkills));

        private void AddNonCriticalFailure(decimal p, int r, Skills usedSkills) => AddOrUpdateOutcomes(p, new Tuple<bool, int, Skills>(true, r, usedSkills));

        private void AddOrUpdateOutcomes(decimal p, Tuple<bool, int, Skills> key)
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
