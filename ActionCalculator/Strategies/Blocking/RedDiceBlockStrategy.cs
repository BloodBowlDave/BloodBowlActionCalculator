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
        private readonly ID6 _iD6;
        private readonly IRollOutcomeHelper _rollOutcomeHelper;

        public RedDiceBlockStrategy(IActionMediator actionMediator, IProHelper proHelper, IBrawlerHelper brawlerHelper, 
            ID6 iD6, IRollOutcomeHelper rollOutcomeHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _brawlerHelper = brawlerHelper;
            _iD6 = iD6;
            _rollOutcomeHelper = rollOutcomeHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var ((lonerSuccess, proSuccess, _), action, i) = playerAction;
            
            var numberOfSuccessfulResults = action.NumberOfSuccessfulResults;
            var successfulValues = _iD6.Rolls().Take(numberOfSuccessfulResults).ToList();
            var nonCriticalFailureValues = _iD6.Rolls().Skip(numberOfSuccessfulResults).Take(action.NumberOfNonCriticalFailures).ToList();
            var numberOfDice = -action.NumberOfDice;
            var rolls = _iD6.Rolls(numberOfDice);
            var rollCount = rolls.Count;
            
            var successOnOneDie = action.SuccessOnOneDie;

            var useBrawler = _brawlerHelper.UseBrawler(r, playerAction, usedSkills);
            var success = action.Success;
            var usePro = _proHelper.UsePro(playerAction, r, usedSkills, successOnOneDie, success);

            var rollOutcomes = _rollOutcomeHelper.GetRollOutcomesForRedDice(rolls, successfulValues, nonCriticalFailureValues, numberOfDice, useBrawler, usePro);
            
            p /= rollCount;

            var nonCriticalFailures = rollOutcomes.NonCriticalFailures;
            var nonCriticalFailureAfterReroll = (decimal)nonCriticalFailures / rollCount;
            var failures = rollOutcomes.Failures;
            
            _actionMediator.Resolve(p * (rollOutcomes.Successes + rollOutcomes.BrawlerRolls * successOnOneDie), r, i, usedSkills);
            _actionMediator.Resolve(p * failures * lonerSuccess * success, r - 1, i, usedSkills);
            _actionMediator.Resolve(p * failures * lonerSuccess * nonCriticalFailureAfterReroll, r - 1, i, usedSkills, true);

            var brawlerAndProSuccess = rollOutcomes.BrawlerAndProRolls * proSuccess * successOnOneDie * successOnOneDie;
            var successAfterPro = rollOutcomes.ProRolls * proSuccess * successOnOneDie;
            _actionMediator.Resolve(p * (brawlerAndProSuccess + successAfterPro), r, i, usedSkills | Skills.Pro);

            var rerollFailure = 1 - lonerSuccess;

            if (action.RerollNonCriticalFailure && r > 0)
            {
                _actionMediator.Resolve(p * nonCriticalFailures * lonerSuccess * success, r - 1, i, usedSkills);
                _actionMediator.Resolve(p * nonCriticalFailures * rerollFailure, r - 1, i, usedSkills, true);
                _actionMediator.Resolve(p * nonCriticalFailures * lonerSuccess * nonCriticalFailureAfterReroll, r - 1, i, usedSkills, true);
            }
            else
            {
                _actionMediator.Resolve(p * nonCriticalFailures, r, i, usedSkills, true);
            }
        }
    }
}
