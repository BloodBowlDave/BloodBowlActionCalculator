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
        private readonly Abstractions.ID6 _iD6;

        public BlockStrategy(IActionMediator actionMediator, IProHelper proHelper, IBrawlerHelper brawlerHelper, Abstractions.ID6 iD6)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
            _brawlerHelper = brawlerHelper;
            _iD6 = iD6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (player, action, i) = playerAction;
            var (rerollSuccess, proSuccess, _) = player;
            
            var successfulRolls = new[] { 6, 5, 4, 3, 2, 1 }.Take(action.NumberOfSuccessfulResults);
            var numberOfDice = action.NumberOfDice;
            var rolls = _iD6.Rolls(Math.Abs(numberOfDice));
            var successes = rolls.Where(x => x.Intersect(successfulRolls).Any()).ToList();
            var failures = rolls.Where(x => !x.Intersect(successfulRolls).Any()).ToList();
            var successCount = successes.Count;
            var rollCount = rolls.Count;

            p /= rollCount;

            _actionMediator.Resolve(p * successCount, r, i, usedSkills);

            var successOnOneDie = action.SuccessOnOneDie;
            var success = (decimal)successCount / rollCount;

            if (_brawlerHelper.CanUseBrawler(r, playerAction, usedSkills))
            {
                var brawlerCount = failures.Count(x => x.Contains(2));
                _actionMediator.Resolve(p * brawlerCount * successOnOneDie, r, i, usedSkills);

                var failureLessBrawlerCount = rollCount - successCount - brawlerCount;
                var brawlerFailure = brawlerCount * (1 - successOnOneDie);

                if (_proHelper.CanUsePro(playerAction, r, usedSkills, successOnOneDie, success))
                {
                    usedSkills |= Skills.Pro;

                    _actionMediator.Resolve(p * proSuccess * failureLessBrawlerCount * successOnOneDie, r, i, usedSkills);

                    numberOfDice--;
                    if (numberOfDice > 0)
                    {
                        _actionMediator.Resolve(p * brawlerFailure * proSuccess * successOnOneDie, r, i, usedSkills);
                    }

                    if (r == 0 || numberOfDice < 1)
                    {
                        return;
                    }
                    
                    var successAfterReroll = GetSuccessAfterReroll(successOnOneDie, numberOfDice);

                    _actionMediator.Resolve(p * failureLessBrawlerCount * proSuccess * (1 - successOnOneDie) * successAfterReroll * rerollSuccess, r - 1, i, usedSkills);
                    _actionMediator.Resolve(p * failureLessBrawlerCount * (1 - proSuccess) * successAfterReroll * rerollSuccess, r - 1, i, usedSkills);

                    numberOfDice--;
                    if (numberOfDice > 0)
                    {
                        _actionMediator.Resolve(p * brawlerFailure * proSuccess * (1 - successOnOneDie) * successOnOneDie * rerollSuccess, r - 1, i, usedSkills);
                        _actionMediator.Resolve(p * brawlerFailure * (1 - proSuccess) * successOnOneDie * rerollSuccess, r - 1, i, usedSkills);
                    }

                    return;
                }

                if (r == 0)
                {
                    return;
                }

                _actionMediator.Resolve(p * failureLessBrawlerCount * success * rerollSuccess, r - 1, i, usedSkills);

                numberOfDice--;
                if (numberOfDice > 0)
                {
                    _actionMediator.Resolve(p * brawlerFailure * successOnOneDie * rerollSuccess, r - 1, i, usedSkills);
                }
                return;
            }

            var failureCount = failures.Count;

            if (_proHelper.CanUsePro(playerAction, r, usedSkills, successOnOneDie, success))
            {
                usedSkills |= Skills.Pro;

                _actionMediator.Resolve(p * proSuccess * failureCount * successOnOneDie, r, i, usedSkills);

                if (r == 0)
                {
                    return;
                }

                _actionMediator.Resolve(p * failureCount * (1 - proSuccess + proSuccess * (1 - successOnOneDie)) * successOnOneDie * rerollSuccess, r - 1, i, usedSkills);
                return;
            }

            if (r > 0)
            {
                _actionMediator.Resolve(p * failures.Count * rerollSuccess * success, r - 1, i, usedSkills);
            }
        }

        private static decimal GetSuccessAfterReroll(decimal successOnOneDie, int numberOfDice) => 
            1m - (decimal) Math.Pow(decimal.ToDouble(1m - successOnOneDie), numberOfDice);
    }
}
