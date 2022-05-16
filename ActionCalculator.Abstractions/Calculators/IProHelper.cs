namespace ActionCalculator.Abstractions.Calculators
{
    public interface IProHelper
    {
        bool UsePro(PlayerAction playerAction, int r, Skills usedSkills, decimal? successWithPro = null, decimal? successWithReroll = null);
    }
}