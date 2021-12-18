namespace ActionCalculator.Abstractions.ProbabilityCalculators.Block
{
	public interface IBrawlerCalculator
	{
		decimal FailButRollBothDown(Action action);
		bool UseBrawler(int r, PlayerAction playerAction);
	}
}