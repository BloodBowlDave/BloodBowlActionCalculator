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
                skillsWithValues.Aggregate(Skills.None, (current, skill) => current | skill.Item1),
                GetSkillValue(skillsWithValues, Skills.Loner),
                GetSkillValue(skillsWithValues, Skills.BreakTackle),
                GetSkillValue(skillsWithValues, Skills.MightyBlow),
                GetSkillValue(skillsWithValues, Skills.DirtyPlayer),
                GetSkillValue(skillsWithValues, Skills.Incorporeal));
        }

        private static List<Tuple<Skills, int>> GetSkillsWithValues(string skillsInput) =>
            skillsInput.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(GetSkillAndRoll)
                .Select(x =>
                    new Tuple<Skills, int>(x.Item1.GetValueFromDescription<Skills>(), x.Item2))
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

        private static int GetSkillValue(IEnumerable<Tuple<Skills, int>> skills, Skills skill) =>
            skills.SingleOrDefault(x => x.Item1 == skill && x.Item2 > 1)?.Item2 ?? 1;
    }
}
