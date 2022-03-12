namespace ActionCalculator.Abstractions.Calculators
{
	public interface IProCalculator
	{
		bool UsePro(PlayerAction playerAction, int r, Skills usedSkills);
	}
}