namespace ActionCalculator.Abstractions.ProbabilityCalculators.Block
{
	public interface IBrawlerCalculator
	{
		decimal ProbabilityCanUseBrawler(Action action);
		bool UseBrawler(int r, PlayerAction playerAction);
		decimal ProbabilityCanUseBrawlerAndPro(Action action);
	}
}