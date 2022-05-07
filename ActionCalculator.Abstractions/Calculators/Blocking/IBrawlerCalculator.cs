namespace ActionCalculator.Abstractions.Calculators.Blocking
{
    public interface IBrawlerCalculator
    {
        decimal ProbabilityCanUseBrawler(Action action);
        bool UseBrawler(int r, PlayerAction playerAction);
        bool UseBrawlerAndPro(int r, PlayerAction playerAction, Skills usedSkills);
        decimal ProbabilityCanUseBrawlerAndPro(Action action);
    }
}