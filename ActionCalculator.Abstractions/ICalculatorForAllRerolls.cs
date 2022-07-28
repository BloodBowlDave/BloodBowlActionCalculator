using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface ICalculatorForAllRerolls
    {
        public IEnumerable<CalculationResult> CalculateForAllRerolls(string playerActionsString);
    }
}