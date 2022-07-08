using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface IActionMediator
    {
        decimal[] Calculate(Calculation calculation);
        void Resolve(decimal p, int r, int i, Skills usedSkills, bool nonCriticalFailure = false);
    }
}
