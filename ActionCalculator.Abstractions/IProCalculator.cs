namespace ActionCalculator.Abstractions
{
	public interface IProCalculator
	{
		bool UsePro(PlayerAction playerAction, int r, Skills usedSkills);
	}
}