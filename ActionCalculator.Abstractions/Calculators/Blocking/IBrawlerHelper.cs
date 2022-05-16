namespace ActionCalculator.Abstractions.Calculators.Blocking
{
    public interface IBrawlerHelper
    {
        bool UseBrawler(int r, PlayerAction playerAction, Skills usedSkills);
        bool UseBrawlerAndPro(int r, PlayerAction playerAction, Skills usedSkills);
    }
}