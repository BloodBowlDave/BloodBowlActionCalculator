namespace ActionCalculator.Abstractions.Calculators
{
    public interface IProHelper
    {
        bool CanUsePro(PlayerAction playerAction, int r, Skills usedSkills, decimal? successWithPro = null, decimal? successWithReroll = null);
    }
}