using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using ActionCalculator.Utilities;

namespace ActionCalculator
{
    public class PlayerBuilder : IPlayerBuilder
    {
        public Player Build(string skillsInput)
        {
            var skillsWithValues = GetSkillsWithValues(skillsInput);

            return new Player(Guid.NewGuid(),
                skillsWithValues.Aggregate(CalculatorSkills.None, (current, skill) => current | skill.Item1),
                GetSkillValue(skillsWithValues, CalculatorSkills.Loner),
                GetSkillValue(skillsWithValues, CalculatorSkills.BreakTackle),
                GetSkillValue(skillsWithValues, CalculatorSkills.MightyBlow),
                GetSkillValue(skillsWithValues, CalculatorSkills.DirtyPlayer),
                GetSkillValue(skillsWithValues, CalculatorSkills.Incorporeal));
        }

        private static List<Tuple<CalculatorSkills, int>> GetSkillsWithValues(string skillsInput) =>
            skillsInput.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(GetSkillAndRoll)
                .Select(x =>
                    new Tuple<CalculatorSkills, int>(x.Item1.GetValueFromDescription<CalculatorSkills>(), x.Item2))
                .ToList();

        private static Tuple<string, int> GetSkillAndRoll(string input)
        {
            var last = input.Last();

            return last is '1' or '2' or '3' or '4' or '5' or '6'
                ? new Tuple<string, int>(input.Remove(input.Length - 1), int.Parse(last.ToString()))
                : input is "L"
                    ? new Tuple<string, int>(input, 4)
                    : input is "P"
                        ? new Tuple<string, int>(input, 3)
                        : new Tuple<string, int>(input, 0);
        }

        private static int GetSkillValue(IEnumerable<Tuple<CalculatorSkills, int>> skills, CalculatorSkills skill) =>
            skills.SingleOrDefault(x => x.Item1 == skill && x.Item2 > 1)?.Item2 ?? 1;
    }
}
