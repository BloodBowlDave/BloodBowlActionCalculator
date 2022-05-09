using ActionCalculator.Utilities;

namespace ActionCalculator.Abstractions
{
    public class Player
    {
        public Player()
        {
            Id = Guid.NewGuid();
            Skills = Skills.None;
            RerollSuccess = 1;
            BreakTackleValue = 0;
            MightyBlowValue = 0;
            DirtyPlayerValue = 0;
            ProSuccess = 2m / 3;
        }

        public Player(Skills skills, int lonerValue, int breakTackleValue, int mightyBlowValue, int dirtyPlayerValue, int incorporealValue)
        {
            Id = Guid.NewGuid();
            Skills = skills;
            RerollSuccess = (7m - lonerValue) / 6;
            LonerValue = lonerValue;
            BreakTackleValue = skills.Contains(Skills.Incorporeal) ? incorporealValue : breakTackleValue;
            MightyBlowValue = mightyBlowValue;
            DirtyPlayerValue = dirtyPlayerValue;
            ProSuccess = skills.Contains(Skills.ConsummateProfessional) ? 1m : 2m / 3;
        }


        public Guid Id { get; }
        private Skills Skills { get; }
        public decimal RerollSuccess { get; }
        private int LonerValue { get; }
        public int BreakTackleValue { get; }
        public int MightyBlowValue { get; }
        public int DirtyPlayerValue { get; }
        public decimal ProSuccess { get; }

        public bool CanUseSkill(Skills skill, Skills usedSkills)
        {
            var underlyingSkill = skill switch
            {
                Skills.OldPro => Skills.Pro,
                Skills.Incorporeal => Skills.BreakTackle,
                Skills.ConsummateProfessional => Skills.Pro,
                _ => skill
            };

            return Skills.Contains(skill) && !usedSkills.Contains(underlyingSkill);
        }

        public override string ToString() =>
            string.Join(',', Skills.ToEnumerable(Skills.None)
                .Select(x => x.GetDescriptionFromValue() + GetSkillRoll(x))
                .OrderBy(x => x));

        private string GetSkillRoll(Skills skill) =>
            skill switch
            {
                Skills.Loner => LonerValue.ToString(),
                Skills.DirtyPlayer => DirtyPlayerValue.ToString(),
                Skills.MightyBlow => MightyBlowValue.ToString(),
                Skills.BreakTackle => BreakTackleValue.ToString(),
                Skills.Incorporeal => BreakTackleValue.ToString(),
                _ => ""
            };

        public bool HasSkills() => Skills != Skills.None;

        public void Deconstruct(out decimal useReroll, out decimal proSuccess, out Func<Skills, Skills, bool> canUseSkill)
        {
            useReroll = RerollSuccess;
            proSuccess = ProSuccess;
            canUseSkill = CanUseSkill;
        }
    }
}