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
            var (player, action, i) = playerAction;
            var (rerollSuccess, proSuccess, _) = player;
            
            var successfulValues = new[] { 6, 5, 4, 3, 2, 1 }.Take(action.NumberOfSuccessfulResults);
            var numberOfDice = action.NumberOfDice;
            var rolls = _iD6.Rolls(-numberOfDice);
            var successes = rolls.Where(x => x.All(y => successfulValues.Contains(y))).ToList();
            var failures = rolls.Where(x => !x.All(y => successfulValues.Contains(y))).ToList();
            var successCount = successes.Count;
            var rollCount = rolls.Count;

            p /= rollCount;

            _actionMediator.Resolve(p * successCount, r, i, usedSkills);

            var successOnOneDie = action.SuccessOnOneDie;
            var success = (decimal)successCount / rollCount;

            if (_brawlerHelper.UseBrawlerAndPro(r, playerAction, usedSkills))
            {
                //var brawlerAndProCount = failures.Where(x => )
            }

            if (_brawlerHelper.CanUseBrawler(r, playerAction, usedSkills))
            {
                var brawlerCount = failures.Count(x => x.All(y => successfulValues.Contains(y) || y == 2) && x.Count(y => y == 2) == 1);

                _actionMediator.Resolve(p * brawlerCount * successOnOneDie, r, i, usedSkills);

                var brawlerFailure = brawlerCount * (1 - successOnOneDie);

                //if (_proHelper.CanUsePro(playerAction, r, usedSkills, successOnOneDie, success))
                //{
                //    var proCount = failures.Count(x => x.Count(y => !successfulValues.Contains(y)) == 1) - brawlerCount;

                //    usedSkills |= Skills.Pro;

                //    _actionMediator.Resolve(p * proSuccess * proCount * successOnOneDie, r, i, usedSkills);

                //    numberOfDice--;
                //    if (numberOfDice > 0)
                //    {
                //        _actionMediator.Resolve(p * brawlerFailure * proSuccess * successOnOneDie, r, i, usedSkills);
                //    }

                //    if (r == 0 || numberOfDice < 1)
                //    {
                //        return;
                //    }

                //    var successAfterReroll = GetSuccessAfterReroll(successOnOneDie, numberOfDice);

                //    p *= rerollSuccess;

                //    _actionMediator.Resolve(p * proCount * successAfterReroll * (proSuccess * (1 - successOnOneDie) + (1 - proSuccess)), r - 1, i, usedSkills);

                //    numberOfDice--;
                //    if (numberOfDice > 0)
                //    {
                //        _actionMediator.Resolve(p * brawlerFailure * successOnOneDie * (proSuccess * (1 - successOnOneDie) + (1 - proSuccess)), r - 1, i, usedSkills);
                //    }

                //    return;
                //}

                if (r == 0)
                {
                    return;
                }

                var failureLessBrawlerCount = rollCount - successCount - brawlerCount;
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
