using ActionCalculator.Models;

namespace ActionCalculator.Abstractions.Strategies
{
    public interface IActionStrategy
    {
        void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false);
    }
}