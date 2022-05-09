namespace ActionCalculator.Abstractions.Calculators
{
    public interface IProHelper
    {
        bool CanUsePro(PlayerAction playerAction, int r, Skills usedSkills, decimal? successOnOneDie = null, decimal? successAfterReroll = null);
    }
}