using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Strategies.Blocking
{
    public class RedDiceBlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly IBrawlerHelper _brawlerHelper;
        private readonly Abstractions.ID6 _iD6;

        public RedDiceBlockStrategy(IActionMediator actionMediator, IProHelper proHelper, IBrawlerHelper brawlerHelper, Abstractions.ID6 iD6)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _brawlerHelper = brawlerHelper;
            _iD6 = iD6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((rerollSuccess, proSuccess, _), action, i) = playerAction;
            
            var successfulValues = new[] { 6, 5, 4, 3, 2, 1 }.Take(action.NumberOfSuccessfulResults).ToList();
            var numberOfDice = -action.NumberOfDice;
            var rolls = _iD6.Rolls(numberOfDice);
            var failures = rolls.Where(x => !x.All(y => successfulValues.Contains(y))).ToList();
            var successCount = rolls.Count(x => x.All(y => successfulValues.Contains(y)));
            var rollCount = rolls.Count;
            
            var successOnOneDie = action.SuccessOnOneDie;

            var brawlerAndProCount = GetBrawlerAndProCount(r, playerAction, usedSkills, failures, successfulValues, numberOfDice);
            var brawlerCount = GetBrawlerCount(r, playerAction, usedSkills, failures, successfulValues, numberOfDice);
            var proCount = GetProCount(r, playerAction, usedSkills, successOnOneDie, failures, successfulValues, numberOfDice, brawlerCount);

            var brawlerAndProSuccess = brawlerAndProCount * proSuccess * successOnOneDie * successOnOneDie;
            var successAfterPro = proCount * proSuccess * successOnOneDie;

            p /= rollCount;
            _actionMediator.Resolve(p * (brawlerAndProSuccess + successAfterPro), r, i, usedSkills | Skills.Pro);
            _actionMediator.Resolve(p * (successCount + brawlerCount * successOnOneDie), r, i, usedSkills);
            _actionMediator.Resolve(p * (failures.Count - brawlerCount - proCount - brawlerAndProCount) * rerollSuccess * action.Success, r - 1, i, usedSkills);
        }

        private int GetBrawlerAndProCount(int r, PlayerAction playerAction, Skills usedSkills, IEnumerable<List<int>> failures, 
            IEnumerable<int> successfulValues, int numberOfDice) =>
            _brawlerHelper.CanUseBrawlerAndPro(r, playerAction, usedSkills) 
                ? failures.Count(x => x.Count(successfulValues.Contains) == numberOfDice - 2 && x.Contains(2)) 
                : 0;

        private int GetBrawlerCount(int r, PlayerAction playerAction, Skills usedSkills, 
            IEnumerable<List<int>> failures, ICollection<int> successfulValues, int numberOfDice) =>
            _brawlerHelper.CanUseBrawler(r, playerAction, usedSkills) 
                ? failures.Count(x => x.Count(successfulValues.Contains) == numberOfDice - 1 && x.Contains(2)) 
                : 0;

        private int GetProCount(int r, PlayerAction playerAction, Skills usedSkills, decimal successOnOneDie,
            IEnumerable<List<int>> failures, ICollection<int> successfulValues, int numberOfDice, int brawlerCount) =>
            _proHelper.CanUsePro(playerAction, r, usedSkills, successOnOneDie, playerAction.Action.Success)
                ? failures.Count(x => x.Count(successfulValues.Contains) == numberOfDice - 1) - brawlerCount
                : 0;
    }
}
