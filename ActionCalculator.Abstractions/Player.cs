namespace ActionCalculator.Abstractions
{
	public class Player
	{
		public Player(int index)
		{
			Index = index;
		}

		public Player(int index, Skills skills, double? lonerSuccess, double? proSuccess)
		{
			Index = index;
			Skills = skills;
			LonerSuccess = lonerSuccess;
			ProSuccess = proSuccess;
		}

		public int Index { get; }
		public Skills Skills { get; }
		public double? ProSuccess { get; }
		public double? LonerSuccess { get; }
	}
}