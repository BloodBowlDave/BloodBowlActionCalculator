using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface ICalculator
    {
        public CalculationResult Calculate(string playerActionsString);
    }
}