using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Strategies.Blocking
{
    public class BlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;
        private readonly IBrawlerHelper _brawlerHelper;
        private readonly IRollOutcomeHelper _rollOutcomeHelper;
        private readonly ID6 _iD6;

        public BlockStrategy(IActionMediator actionMediator, IProHelper proHelper, IBrawlerHelper brawlerHelper, 
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
            var (player, action, i) = playerAction;
            var (lonerSuccess, proSuccess, _) = player;

            var numberOfSuccessfulResults = action.NumberOfSuccessfulResults;
            var successfulValues = _iD6.Rolls().Take(numberOfSuccessfulResults).ToList();
            var nonCriticalFailureValues = _iD6.Rolls().Skip(numberOfSuccessfulResults).Take(action.NumberOfNonCriticalFailures).ToList();
            var numberOfDice = action.NumberOfDice;
            var rolls = _iD6.Rolls(numberOfDice);

            var successOnOneDie = action.SuccessOnOneDie;
            var nonCriticalFailureOnOneDie = action.NonCriticalFailureOnOneDie;
            var useBrawler = _brawlerHelper.UseBrawler(r, playerAction, usedSkills);
            var success = action.Success;
            var usePro = _proHelper.UsePro(playerAction, r, usedSkills, successOnOneDie, success);

            var rollOutcomes = _rollOutcomeHelper.GetRollOutcomes(rolls, successfulValues, nonCriticalFailureValues, numberOfDice, useBrawler, usePro);

            p /= rolls.Count;

            _actionMediator.Resolve(p * (rollOutcomes.Successes + rollOutcomes.BrawlerRolls * successOnOneDie), r, i, usedSkills);

            var failures = rollOutcomes.Failures;
            var nonCriticalFailures = rollOutcomes.NonCriticalFailures;

            _actionMediator.Resolve(p * lonerSuccess * failures * success, r - 1, i, usedSkills);

            var nonCriticalFailureAfterReroll = (decimal)(nonCriticalFailures + rollOutcomes.ProNonCriticalFailures) / rolls.Count;
            var rerollFailure = 1 - lonerSuccess;

            var rerollNonCriticalFailure = action.RerollNonCriticalFailure;
            if (rerollNonCriticalFailure && r > 0)
            {
                _actionMediator.Resolve(p * nonCriticalFailures * lonerSuccess * success, r - 1, i, usedSkills);
                _actionMediator.Resolve(p * ((failures + nonCriticalFailures) * lonerSuccess * nonCriticalFailureAfterReroll + nonCriticalFailures * rerollFailure), r - 1, i, usedSkills, true);
            }
            else
            {
                _actionMediator.Resolve(p * nonCriticalFailures, r, i, usedSkills, true);
                _actionMediator.Resolve(p * failures * lonerSuccess * nonCriticalFailureAfterReroll, r - 1, i, usedSkills, true);
            }
            
            usedSkills |= Skills.Pro;

            var failureOnOnDie = 1 - successOnOneDie - nonCriticalFailureOnOneDie;
            var proRolls = rollOutcomes.ProRolls;
            var brawlerAndProRolls = rollOutcomes.BrawlerAndProRolls;

            _actionMediator.Resolve(p * proRolls * proSuccess * successOnOneDie, r, i, usedSkills);
            _actionMediator.Resolve(p * proRolls * proSuccess * nonCriticalFailureOnOneDie, r, i, usedSkills, true);
            _actionMediator.Resolve(p * brawlerAndProRolls * failureOnOnDie * proSuccess * successOnOneDie, r, i, usedSkills);

            var proFailure = 1 - proSuccess;
            var proNonCriticalFailures = rollOutcomes.ProNonCriticalFailures;

            if (rerollNonCriticalFailure)
            {
                var proMultipleDiceNonCriticalFailures = rollOutcomes.ProMultipleDiceNonCriticalFailures;
                _actionMediator.Resolve(p * proNonCriticalFailures * proSuccess * successOnOneDie, r, i, usedSkills);
                _actionMediator.Resolve(p * proNonCriticalFailures * proFailure, r, i, usedSkills, true);
                _actionMediator.Resolve(p * proNonCriticalFailures * proSuccess * nonCriticalFailureOnOneDie, r, i, usedSkills, true);
                _actionMediator.Resolve(p * proMultipleDiceNonCriticalFailures * proSuccess * failureOnOnDie, r, i, usedSkills, true);
            }
            else
            {
                _actionMediator.Resolve(p * proNonCriticalFailures, r, i, usedSkills, true);
            }

        }
    }
}
