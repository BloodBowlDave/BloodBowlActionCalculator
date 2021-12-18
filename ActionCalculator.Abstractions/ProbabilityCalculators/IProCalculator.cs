namespace ActionCalculator.Abstractions.ProbabilityCalculators
{
	public interface IProCalculator
	{
		bool UsePro(PlayerAction playerAction, int r, Skills usedSkills);
	}
}