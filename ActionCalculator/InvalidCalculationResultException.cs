using ActionCalculator.Models;

namespace ActionCalculator
{
    public class InvalidCalculationResultException(Calculation calculation, IEnumerable<decimal> results)
        : Exception($"Calculation {calculation} with {calculation.Rerolls} rerolls produced invalid results [{string.Join(", ", results)}]");
}
