using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface IActionMediator
    {
        void Initialise(Calculation context);
        void Resolve(decimal p, int r, int i, Skills usedSkills, bool nonCriticalFailure = false);
    }
}
