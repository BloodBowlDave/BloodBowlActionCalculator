using System;
using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class PlayerParser : IPlayerParser
    {
        public Player Parse(string input)
        {
            var skills = input.Split(',')
                .Select(GetSkillAndRoll)
                .Select(x => 
                    new Tuple<Skills, int>(EnumExtensions.GetValueFromDescription<Skills>(x.Item1), x.Item2))
                .ToList();
            
            return new Player
            {
                Skills = skills.Aggregate(Skills.None, (current, skill) => current | skill.Item1),
                LonerSuccess = SkillSuccess(skills, Skills.Loner),
                ProSuccess = SkillSuccess(skills, Skills.Pro)
            };
        }

        private static double? SkillSuccess(IEnumerable<Tuple<Skills, int>> skillsAndRolls, Skills skill)
        {
            var roll = skillsAndRolls.SingleOrDefault(x => x.Item1 == skill)?.Item2;

            return (7.0 - roll) / 6;
        }

        private static Tuple<string, int> GetSkillAndRoll(string input)
        {
            var last = input.Last();

            return last == '2' || last == '3' || last == '4' || last == '5' || last == '6'
                ? new Tuple<string, int>(input.Remove(input.Length - 1), int.Parse(last.ToString()))
                : new Tuple<string, int>(input, 0);
        }
    }
}
