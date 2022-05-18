namespace ActionCalculator.Abstractions.Calculators.Blocking;

public interface IRollOutcomeHelper
{
    RollOutcomes GetRollOutcomes(List<List<int>> rolls, ICollection<int> successfulValues,
        ICollection<int> nonCriticalFailureValues, int numberOfDice, bool useBrawler, bool usePro);
    RollOutcomes GetRollOutcomesForRedDice(List<List<int>> rolls, ICollection<int> successfulValues,
        ICollection<int> nonCriticalFailureValues, int numberOfDice, bool useBrawler, bool usePro);
}