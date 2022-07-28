using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Abstractions.Strategies.Blocking;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.Blocking
{
    public class BlockStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IBrawlerHelper _brawlerHelper;
        private readonly IProHelper _proHelper;
        private readonly ID6 _d6;
        private Dictionary<Tuple<bool, int, Skills>, decimal> _outcomes = new();
        private List<int> _successfulValues = new();
        private List<int> _nonCriticalFailureValues = new();
        private int _rollsCount;
        private int _rerollCount;
        private bool _useBrawler;
        private bool _canUseBrawler;
        private bool _usePro;
        private bool _rerollNonCriticalFailure;
        private decimal _proSuccess;
        private decimal _lonerSuccess;

        public BlockStrategy(ICalculator calculator, IBrawlerHelper brawlerHelper, IProHelper proHelper, ID6 d6)
        {
            _calculator = calculator;
            _brawlerHelper = brawlerHelper;
            _proHelper = proHelper;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            foreach (var outcome in GetOutcomes(r, playerAction, usedSkills))
            {
                var ((outcomeNonCriticalFailure, rerollsRemaining, outcomeSkillsUsed), pOutcome) = outcome;
                
                _calculator.Resolve(p * pOutcome, rerollsRemaining, i, usedSkills | outcomeSkillsUsed, outcomeNonCriticalFailure);
            }
        }
        
        private Dictionary<Tuple<bool, int, Skills>, decimal> GetOutcomes(int r, PlayerAction playerAction, Skills usedSkills)
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
            var rolls = _d6.Rolls(numberOfDice);
            var success = 1 - (decimal)Math.Pow((double)(1 - successOnOneDie), numberOfDice);

            _rollsCount = rolls.Count;
            _useBrawler = _brawlerHelper.UseBrawler(player, block, r, usedSkills, successOnOneDie, success);
            _canUseBrawler = player.CanUseSkill(Skills.Brawler, usedSkills);
            _usePro = _proHelper.UsePro(player, block, r, usedSkills, successOnOneDie, success);
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

            if (roll.Any(_nonCriticalFailureValues.Contains) && !_rerollNonCriticalFailure)
            {
                if (_canUseBrawler && indexOfBothDown != -1)
                {
                    var rollWithoutBothDown = new List<int>(roll);
                    rollWithoutBothDown.RemoveAt(indexOfBothDown);

                    if (rollWithoutBothDown.Any(_nonCriticalFailureValues.Contains))
                    {
                        Brawler(roll, r, indexOfBothDown);
                    }
                }

                AddOutcome(1m / _rollsCount, r, Skills.None, roll);
                return;
            }
            
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

            if (r == 0)
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
                var proReroll = new List<int>(roll) { [indexOfLowestValue] = i };
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

                var brawlerReroll = new List<int>(roll) { [indexOfBothDown] = i };

                if (!_usePro)
                {
                    continue;
                }

                AddOutcome((1 - _proSuccess) / _rollsCount / 6, r, Skills.Pro, brawlerReroll);
                ProAfterBrawler(brawlerReroll, r, indexOfBothDown);
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
                var proReroll = new List<int>(brawlerReroll) { [indexOfLowestValue] = i };
                AddOutcome(_proSuccess / _rollsCount / 36, r, Skills.Pro, proReroll);
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
