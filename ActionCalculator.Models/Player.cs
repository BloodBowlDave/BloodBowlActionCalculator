using System.Text;
using ActionCalculator.Utilities;

namespace ActionCalculator.Models
{
    public class Player
    {
        public Player()
        {
            Id = Guid.NewGuid();
            Skills = CalculatorSkills.None;
            LonerValue = 4;
            BreakTackleValue = 1;
            MightyBlowValue = 1;
            DirtyPlayerValue = 1;
            ProSuccess = 2m / 3;
        }

        public Player(Guid id, CalculatorSkills skills, int lonerValue, int breakTackleValue, int mightyBlowValue, int dirtyPlayerValue, int incorporealValue)
        {
            Id = id;
            Skills = skills;
            LonerValue = lonerValue;
            BreakTackleValue = skills.Contains(CalculatorSkills.Incorporeal) ? incorporealValue : breakTackleValue;
            MightyBlowValue = mightyBlowValue;
            DirtyPlayerValue = dirtyPlayerValue;
            ProSuccess = skills.Contains(CalculatorSkills.ConsummateProfessional) ? 1m : 2m / 3;
        }


        public Guid Id { get; }
        public CalculatorSkills Skills { get; set; }
        public int LonerValue { get; set; }
        public int BreakTackleValue { get; set; }
        public int MightyBlowValue { get; set; }
        public int DirtyPlayerValue { get; set; }
        public decimal ProSuccess { get; }

        public bool CanUseSkill(CalculatorSkills skill, CalculatorSkills usedSkills)
        {
            var underlyingSkill = skill switch
            {
                CalculatorSkills.OldPro => CalculatorSkills.Pro,
                CalculatorSkills.Incorporeal => CalculatorSkills.BreakTackle,
                CalculatorSkills.ConsummateProfessional => CalculatorSkills.Pro,
                _ => skill
            };

            return Skills.Contains(skill) && !usedSkills.Contains(underlyingSkill);
        }

        public override string ToString() =>
            string.Join(',', Skills.ToEnumerable(CalculatorSkills.None)
                .Select(x => x.GetDescriptionFromValue() + GetSkillRoll(x))
                .OrderBy(x => x));

        private string GetSkillRoll(CalculatorSkills skill) =>
            skill switch
            {
                CalculatorSkills.Loner => LonerValue.ToString(),
                CalculatorSkills.DirtyPlayer => DirtyPlayerValue.ToString(),
                CalculatorSkills.MightyBlow => MightyBlowValue.ToString(),
                CalculatorSkills.BreakTackle => BreakTackleValue.ToString(),
                CalculatorSkills.Incorporeal => BreakTackleValue.ToString(),
                _ => ""
            };

        public bool HasAnySkills() => Skills != CalculatorSkills.None;

        public void Deconstruct(out decimal lonerSuccess, out decimal proSuccess, out Func<CalculatorSkills, CalculatorSkills, bool> canUseSkill)
        {
            lonerSuccess = LonerSuccess();
            proSuccess = ProSuccess;
            canUseSkill = CanUseSkill;
        }

        public decimal LonerSuccess()
        {
            return Skills.Contains(CalculatorSkills.Loner) ? (7m - LonerValue) / 6 : 1;
        }

        public string Description()
        {
            if (Skills == CalculatorSkills.None)
            {
                return "None";
            }

            var sb = new StringBuilder();

            foreach (var skill in Skills.ToEnumerable(CalculatorSkills.None))
            {
                sb.Append($"{skill.ToString().PascalCaseToSpaced()}{GetSkillValue(skill)}, ");
            }

            return sb.Remove(sb.Length - 2, 2).ToString();
        }

        private string GetSkillValue(CalculatorSkills skill) => skill switch
        {
            CalculatorSkills.Loner => " " + LonerValue + "+",
            CalculatorSkills.BreakTackle => " +" + BreakTackleValue,
            CalculatorSkills.DirtyPlayer => " +" + DirtyPlayerValue,
            CalculatorSkills.MightyBlow => " +" + MightyBlowValue,
            _ => ""
        };
    }
}