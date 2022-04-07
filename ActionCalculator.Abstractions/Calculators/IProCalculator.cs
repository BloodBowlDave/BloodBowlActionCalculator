namespace ActionCalculator.Abstractions.Calculators
{
	public interface IProCalculator
	{
		bool UsePro(PlayerAction playerAction, int r, Skills usedSkills, decimal? successOnOneDie = null, decimal? successAfterReroll = null);
	}
}