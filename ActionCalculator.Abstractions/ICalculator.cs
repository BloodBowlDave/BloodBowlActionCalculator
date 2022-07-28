using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface ICalculator
    {
        CalculationResult Calculate(Calculation calculation);
        void Resolve(decimal p, int r, int i, Skills usedSkills, bool nonCriticalFailure = false);
    }
}
