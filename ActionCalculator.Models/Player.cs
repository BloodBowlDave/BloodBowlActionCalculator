using ActionCalculator.Utilities;

namespace ActionCalculator.Models
{
    public class Player
    {
        public Player()
        {
            Id = Guid.NewGuid();
            Skills = Skills.None;
            LonerValue = 4;
            BreakTackleValue = 1;
            MightyBlowValue = 1;
            DirtyPlayerValue = 1;
            ProSuccess = 2m / 3;
        }

        public Player(Guid id, Skills skills, int lonerValue, int breakTackleValue, int mightyBlowValue, int dirtyPlayerValue, int incorporealValue)
        {
            Id = id;
            Skills = skills;
            LonerValue = lonerValue;
            BreakTackleValue = skills.Contains(Skills.Incorporeal) ? incorporealValue : breakTackleValue;
            MightyBlowValue = mightyBlowValue;
            DirtyPlayerValue = dirtyPlayerValue;
            ProSuccess = skills.Contains(Skills.ConsummateProfessional) ? 1m : 2m / 3;
        }


        public Guid Id { get; }
        public Skills Skills { get; set; }
        public int LonerValue { get; set; }
        public int BreakTackleValue { get; set; }
        public int MightyBlowValue { get; set; }
        public int DirtyPlayerValue { get; set; }
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

        public bool HasAnySkills() => Skills != Skills.None;

        public void Deconstruct(out decimal lonerSuccess, out decimal proSuccess, out Func<Skills, Skills, bool> canUseSkill)
        {
            lonerSuccess = LonerSuccess();
            proSuccess = ProSuccess;
            canUseSkill = CanUseSkill;
        }

        public decimal LonerSuccess()
        {
            return Skills.Contains(Skills.Loner) ? (7m - LonerValue) / 6 : 1;
        }
    }
}