using System;

namespace ActionCalculator.Abstractions
{
	public class Player
	{
        public Player()
        {
            Id = Guid.NewGuid();
            Skills = Skills.None;
            LonerSuccess = 1;
            ProSuccess = 0;
            BreakTackleValue = 0;
        }

		public Player(Skills skills, double? lonerSuccess, double? proSuccess, int breakTackleValue)
		{
            Id = Guid.NewGuid();
            Skills = skills;
            LonerSuccess = lonerSuccess != null ? (decimal)lonerSuccess : 1;
            ProSuccess = proSuccess != null ? (decimal)proSuccess : 0;
            BreakTackleValue = breakTackleValue;
        }

		public Guid Id { get; }
		private Skills Skills { get; }
        public decimal LonerSuccess { get; }
        public decimal ProSuccess { get; }
        public int BreakTackleValue { get; }
		public bool HasSkill(Enum skill) => Skills.HasFlag(skill);
	}
}