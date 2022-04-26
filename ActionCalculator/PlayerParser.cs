using System;
using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class PlayerParser : IPlayerParser
    {
        public Player Parse(string skillsInput)
        {
            var skills = skillsInput.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(GetSkillAndRoll)
                .Select(x => 
                    new Tuple<Skills, int>(EnumExtensions.GetValueFromDescription<Skills>(x.Item1), x.Item2))
                .ToList();
            
            return new Player(skills.Aggregate(Skills.None, (current, skill) => current | skill.Item1),
                GetSkillValue(skills, Skills.Loner),
                GetSkillValue(skills, Skills.BreakTackle),
                GetSkillValue(skills, Skills.MightyBlow),
                GetSkillValue(skills, Skills.DirtyPlayer));
        }

        private static double? SkillSuccess(IEnumerable<Tuple<Skills, int>> skillsAndRolls, Skills skill)
        {
            var roll = skillsAndRolls.SingleOrDefault(x => x.Item1 == skill)?.Item2;

            return (7.0 - roll) / 6;
        }

        private static Tuple<string, int> GetSkillAndRoll(string input)
        {
            var last = input.Last();

            return last is '1' or '2' or '3' or '4' or '5' or '6'
                ? new Tuple<string, int>(input.Remove(input.Length - 1), int.Parse(last.ToString()))
                : input is "L"
	                ? new Tuple<string, int>(input, 4) //assume default loner roll is 4
	                : input is "P" 
                        ? new Tuple<string, int>(input, 3)
                        : new Tuple<string, int>(input, 0);
        }

        private static int GetSkillValue(IEnumerable<Tuple<Skills, int>> skills, Skills skill) => 
            skills.SingleOrDefault(x => x.Item1 == skill && x.Item2 > 1)?.Item2 ?? 1;
    }
}
