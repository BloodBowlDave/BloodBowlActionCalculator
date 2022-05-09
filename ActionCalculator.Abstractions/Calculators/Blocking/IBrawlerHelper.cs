namespace ActionCalculator.Abstractions.Calculators.Blocking
{
    public interface IBrawlerHelper
    {
        decimal ProbabilityCanUseBrawler(Action action);
        bool UseBrawler(int r, PlayerAction playerAction, Skills usedSkills);
        bool UseBrawlerAndPro(int r, PlayerAction playerAction, Skills usedSkills);
        decimal ProbabilityCanUseBrawlerAndPro(Action action);
    }
}