using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface ICalculationBuilder
    {
        Calculation Build(string calculation);
    }
}