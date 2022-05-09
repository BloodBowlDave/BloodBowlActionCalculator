namespace ActionCalculator.Abstractions.Calculators.Blocking
{
    public interface IBrawlerHelper
    {
        decimal UseBrawler(Action action);
        bool CanUseBrawler(int r, PlayerAction playerAction, Skills usedSkills);
        bool UseBrawlerAndPro(int r, PlayerAction playerAction, Skills usedSkills);
        decimal ProbabilityCanUseBrawlerAndPro(Action action);
    }
}