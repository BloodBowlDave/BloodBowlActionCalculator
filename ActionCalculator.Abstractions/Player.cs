using ActionCalculator.Utilities;

namespace ActionCalculator.Abstractions
{
	public class Player
	{
        public Player()
        {
            Id = Guid.NewGuid();
            Skills = Skills.None;
            UseReroll = 1;
            BreakTackleValue = 0;
            MightyBlowValue = 0;
            DirtyPlayerValue = 0;
            ProSuccess = 2m / 3;
        }

		public Player(Skills skills, int lonerValue, int breakTackleValue, int mightyBlowValue, int dirtyPlayerValue)
		{
            Id = Guid.NewGuid();
            Skills = skills;
            UseReroll = (7m - lonerValue) / 6;
            LonerValue = lonerValue;
            BreakTackleValue = breakTackleValue;
            MightyBlowValue = mightyBlowValue;
            DirtyPlayerValue = dirtyPlayerValue;
            ProSuccess = skills.HasFlag(Skills.ConsummateProfessional) ? 1m : 2m / 3;
        }

		public Guid Id { get; }
		private Skills Skills { get; }
        public decimal UseReroll { get; }
        private int LonerValue { get; }
        public int BreakTackleValue { get; }
        public int MightyBlowValue { get; }
        public int DirtyPlayerValue { get; }
        public decimal ProSuccess { get; }

        public bool HasSkill(Enum skill) => Skills.HasFlag(skill);

        public override string ToString() =>
            string.Join(',', Skills.ToEnumerable(Skills.None)
                .Select(x => x.GetDescriptionFromValue() + GetSkillRoll(x))
                .OrderBy(x => x));

        private string GetSkillRoll(Skills skill) =>
            skill switch
            {
                Skills.Loner => LonerValue.ToString(),
                Skills.DirtyPlayer => DirtyPlayerValue > 1 ? DirtyPlayerValue.ToString() : "",
                Skills.MightyBlow => MightyBlowValue > 1 ? MightyBlowValue.ToString() : "",
                Skills.BreakTackle => BreakTackleValue.ToString(),
                _ => ""
            };

        public bool HasSkills() => Skills != Skills.None;
    }
}