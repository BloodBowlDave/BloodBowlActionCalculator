using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Strategies.Blocking
{
    public class RollOutcomeHelper : IRollOutcomeHelper
    {
        public RollOutcomes GetRollOutcomes(List<List<int>> rolls, ICollection<int> successfulValues,
            ICollection<int> nonCriticalFailureValues, int numberOfDice, bool useBrawler, bool usePro)
        {
            if (numberOfDice < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfDice));
            }

            var outcomes = new Dictionary<Tuple<int, Skills>, List<Tuple<decimal, List<int>>>>();

            var rollOutcomes = new RollOutcomes();

            foreach (var roll in rolls)
            {
                if (roll.Any(successfulValues.Contains))
                {
                    rollOutcomes.Successes++;
                    continue;
                }

                var nonCriticalFailureCount = roll.Count(nonCriticalFailureValues.Contains);

                if (useBrawler && roll.Contains(2))
                {
                    rollOutcomes.BrawlerRolls++;

                    if (numberOfDice > 1 && usePro)
                    {
                        rollOutcomes.BrawlerAndProRolls++;
                    }

                    continue;
                }
                
                if (usePro)
                {
                    if (nonCriticalFailureCount == 0)
                    {
                        rollOutcomes.ProRolls++;
                    }
                    else
                    {
                        rollOutcomes.ProNonCriticalFailures++;

                        if (numberOfDice > 1)
                        {
                            rollOutcomes.ProMultipleDiceNonCriticalFailures++;
                        }
                    }

                    continue;
                }

                if (nonCriticalFailureCount > 0)
                {
                    rollOutcomes.NonCriticalFailures++;
                    continue;
                }

                rollOutcomes.Failures++;
            }

            return rollOutcomes;
        }

        public RollOutcomes GetRollOutcomesForRedDice(List<List<int>> rolls, ICollection<int> successfulValues, 
            ICollection<int> nonCriticalFailureValues, int numberOfDice, bool useBrawler, bool usePro)
        {
            if (numberOfDice < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfDice));
            }

            var rollOutcomes = new RollOutcomes();

            foreach (var roll in rolls)
            {
                var successfulValueCount = roll.Count(successfulValues.Contains);

                if (successfulValueCount == numberOfDice)
                {
                    rollOutcomes.Successes++;
                    continue;
                }

                var nonCriticalFailureCount = roll.Count(nonCriticalFailureValues.Contains);
                var rolledBothDown = roll.Contains(2);

                if (successfulValueCount == numberOfDice - 1)
                {
                    if (useBrawler && rolledBothDown)
                    {
                        rollOutcomes.BrawlerRolls++;
                    }
                    else if (usePro)
                    {
                        rollOutcomes.ProRolls++;

                        if (nonCriticalFailureCount == 1)
                        {
                            rollOutcomes.ProNonCriticalFailures++;
                        }
                    }
                    else if (nonCriticalFailureCount == 1)
                    {
                        rollOutcomes.NonCriticalFailures++;
                    }
                    else
                    {
                        rollOutcomes.Failures++;
                    }
                    
                    continue;
                }
                
                if (successfulValueCount == numberOfDice - 2)
                {
                    if (usePro && useBrawler && rolledBothDown)
                    {
                        rollOutcomes.BrawlerAndProRolls++;
                    }
                    else if (nonCriticalFailureCount == 2)
                    {
                        rollOutcomes.NonCriticalFailures++;
                    }
                    else
                    {
                        rollOutcomes.Failures++;
                    }

                    continue;
                }

                if (nonCriticalFailureCount == numberOfDice)
                {
                    rollOutcomes.NonCriticalFailures++;
                }
                else
                {
                    rollOutcomes.Failures++;
                }
            }

            return rollOutcomes;
        }
    }
}
