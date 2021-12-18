using System;

namespace ActionCalculator.Abstractions
{
	public class Player
	{
		public Player(int index, Skills skills, double? lonerSuccess, double? proSuccess)
		{
			Index = index;
			Skills = skills;
            LonerSuccess = lonerSuccess != null ? (decimal)lonerSuccess : 1;
			ProSuccess = proSuccess != null ? (decimal)proSuccess : 0;
		}

		public int Index { get; }
		private Skills Skills { get; }
        public decimal LonerSuccess { get; }
        public decimal ProSuccess { get; }
		public bool HasSkill(Enum skill) => Skills.HasFlag(skill);
	}
}