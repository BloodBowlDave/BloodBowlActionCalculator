namespace ActionCalculator.Abstractions.Calculators
{
    public interface ICalculator
    {
        void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false);
    }
}