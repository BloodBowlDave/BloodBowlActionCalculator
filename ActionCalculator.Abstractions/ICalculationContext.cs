using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface ICalculationContext
    {
        Season Season { get; set; }
    }
}
