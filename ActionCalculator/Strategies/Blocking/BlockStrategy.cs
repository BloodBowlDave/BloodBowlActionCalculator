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

            var d6 = new[] { 6, 5, 4, 3, 2, 1 };
            var numberOfSuccessfulResults = action.NumberOfSuccessfulResults;
            var successfulValues = d6.Take(numberOfSuccessfulResults).ToList();
            var nonCriticalFailureValues = d6.Skip(numberOfSuccessfulResults).Take(action.NumberOfNonCriticalFailures).ToList();
            var numberOfDice = action.NumberOfDice;
            var rolls = _iD6.Rolls(numberOfDice);

            var successOnOneDie = action.SuccessOnOneDie;
            var failureOnOnDie = 1 - successOnOneDie;
            var nonCriticalFailureOnOneDie = action.NonCriticalFailureOnOneDie;
            var useBrawler = _brawlerHelper.UseBrawler(r, playerAction, usedSkills);
            var success = action.Success;
            var usePro = _proHelper.UsePro(playerAction, r, usedSkills, successOnOneDie, success);

            var rerollNonCriticalFailure = action.RerollNonCriticalFailure && r > 0;
            var rollOutcomes = GetRollOutcomes(rolls, successfulValues, nonCriticalFailureValues, numberOfDice, useBrawler, usePro, rerollNonCriticalFailure);

            var (successes, brawlerSuccesses, proSuccesses, failures, brawlerFailureProSuccesses, brawlerFailures, 
                brawlerAndProFailures, proFailures, nonCriticalFailures) = rollOutcomes;
            
            p /= rolls.Count;

            _actionMediator.Resolve(p * (successes + brawlerSuccesses * successOnOneDie), r, i, usedSkills);

            var successAfterReroll = GetSuccessAfterReroll(successOnOneDie, numberOfDice - 1);
            
            _actionMediator.Resolve(p * rerollSuccess * brawlerFailures * failureOnOnDie * successAfterReroll, r - 1, i, usedSkills);
            _actionMediator.Resolve(p * rerollSuccess * failures * success, r - 1, i, usedSkills);
            
            var nonCriticalFailureAfterReroll = GetNonCriticalFailureAfterReroll(successOnOneDie, nonCriticalFailureOnOneDie, numberOfDice);

            _actionMediator.Resolve(p * nonCriticalFailures, r, i, usedSkills, true);
            _actionMediator.Resolve(p * failures * rerollSuccess * nonCriticalFailureAfterReroll, r - 1, i, usedSkills, true);

            usedSkills |= Skills.Pro;

            _actionMediator.Resolve(p * proSuccess * successOnOneDie * (proSuccesses + brawlerFailureProSuccesses * failureOnOnDie), r, i, usedSkills);
            
            var proFailureOrOneDieFailure = 1 - proSuccess + proSuccess * failureOnOnDie;
            
            _actionMediator.Resolve(p * rerollSuccess * proFailureOrOneDieFailure * (proFailures * successAfterReroll + brawlerAndProFailures * failureOnOnDie * successOnOneDie), r - 1, i, usedSkills);
        }

        private static RollOutcomes GetRollOutcomes(List<List<int>> rolls, ICollection<int> successfulValues,
            ICollection<int> nonCriticalFailureValues, int numberOfDice, bool useBrawler, bool usePro, bool rerollNonCriticalFailure)
        {
            int successes = 0,
                brawlerSuccesses = 0,
                proSuccesses = 0,
                failures = 0,
                brawlerFailureProSuccesses = 0,
                brawlerFailures = 0,
                brawlerAndProFailures = 0,
                proFailures = 0,
                nonCriticalFailures = 0;

            foreach (var roll in rolls)
            {
                if (roll.Any(successfulValues.Contains))
                {
                    successes++;
                }
                else if (useBrawler && roll.Contains(2))
                {
                    brawlerSuccesses++;

                    if (numberOfDice == 1)
                    {
                        continue;
                    }

                    if (usePro)
                    {
                        brawlerFailureProSuccesses++;

                        if (numberOfDice == 3)
                        {
                            brawlerAndProFailures++;
                        }
                    }
                    else
                    {
                        brawlerFailures++;
                    }
                }
                else if (usePro)
                {
                    proSuccesses++;

                    if (numberOfDice > 1)
                    {
                        proFailures++;
                    }
                }
                else
                {
                    if (roll.Any(nonCriticalFailureValues.Contains) && !rerollNonCriticalFailure)
                    {
                        nonCriticalFailures++;
                    }
                    else
                    {
                        failures++;
                    }

                }
            }

            return new RollOutcomes(successes, brawlerSuccesses, proSuccesses, failures, brawlerFailureProSuccesses, 
                brawlerFailures, brawlerAndProFailures, proFailures, nonCriticalFailures);
        }
        
        private class RollOutcomes
        {
            public RollOutcomes(int successes, int brawlerSuccesses, int proSuccesses, int failures, int brawlerFailureProSuccesses, 
                int brawlerFailures, int brawlerAndProFailures, int proFailures, int nonCriticalFailures)
            {
                Successes = successes;
                BrawlerSuccesses = brawlerSuccesses;
                ProSuccesses = proSuccesses;
                Failures = failures;
                BrawlerFailureProSuccesses = brawlerFailureProSuccesses;
                BrawlerFailures = brawlerFailures;
                BrawlerAndProFailures = brawlerAndProFailures;
                ProFailures = proFailures;
                NonCriticalFailures = nonCriticalFailures;
            }

            public int Successes { get; }
            public int BrawlerSuccesses { get; }
            public int ProSuccesses { get; }
            public int Failures { get; }
            public int BrawlerFailureProSuccesses { get; }
            public int BrawlerFailures { get; }
            public int BrawlerAndProFailures { get; }
            public int ProFailures { get; }
            public int NonCriticalFailures { get; }

            public void Deconstruct(out int successes, out int brawlerSuccesses, out int proSuccesses, out int failures, out int brawlerFailureProSuccesses,
                out int brawlerFailures, out int brawlerAndProFailures, out int proFailures, out int nonCriticalFailures)
            {
                successes = Successes;
                brawlerSuccesses = BrawlerSuccesses;
                proSuccesses = ProSuccesses;
                failures = Failures;
                brawlerFailureProSuccesses = BrawlerFailureProSuccesses;
                brawlerFailures = BrawlerFailures;
                brawlerAndProFailures = BrawlerAndProFailures;
                proFailures = ProFailures;
                nonCriticalFailures = NonCriticalFailures;
            }
        }

        private static decimal GetSuccessAfterReroll(decimal successOnOneDie, int numberOfDice) =>
            numberOfDice > 0 ? 1m - (decimal)Math.Pow(decimal.ToDouble(1m - successOnOneDie), numberOfDice) : 0;

        private static decimal GetNonCriticalFailureAfterReroll(decimal successOnOneDie, decimal nonCriticalFailureOnOneDie, int numberOfDice) =>
            numberOfDice > 0 && nonCriticalFailureOnOneDie > 0
                ? GetSuccessAfterReroll(successOnOneDie, numberOfDice)
                  - (decimal)Math.Pow(decimal.ToDouble(1 - successOnOneDie - nonCriticalFailureOnOneDie), numberOfDice) 
                : 0;
    }
}
