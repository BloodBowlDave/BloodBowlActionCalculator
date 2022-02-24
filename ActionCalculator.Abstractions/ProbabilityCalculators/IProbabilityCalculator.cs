namespace ActionCalculator.Abstractions.ProbabilityCalculators
{
    public interface IProbabilityCalculator
    {
        void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false);
    }
}