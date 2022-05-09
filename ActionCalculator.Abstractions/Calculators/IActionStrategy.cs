namespace ActionCalculator.Abstractions.Calculators
{
    public interface IActionStrategy
    {
        void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false);
    }
}