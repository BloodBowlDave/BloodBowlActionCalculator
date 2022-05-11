namespace ActionCalculator.Abstractions.Calculators.Blocking
{
    public interface IBrawlerHelper
    {
        bool CanUseBrawler(int r, PlayerAction playerAction, Skills usedSkills);
        bool CanUseBrawlerAndPro(int r, PlayerAction playerAction, Skills usedSkills);
    }
}