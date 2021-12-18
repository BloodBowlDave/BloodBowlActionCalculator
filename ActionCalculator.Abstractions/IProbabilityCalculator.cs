namespace ActionCalculator.Abstractions
{
    public interface IProbabilityCalculator
    {
        void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool inaccuratePass = false);
    }
}