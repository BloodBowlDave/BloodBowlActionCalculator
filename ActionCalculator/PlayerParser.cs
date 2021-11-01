﻿using System;
using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class PlayerParser : IPlayerParser
    {
        public Player Parse(string skillsInput, int playerIndex)
        {
            var skills = skillsInput.Split(',')
                .Select(GetSkillAndRoll)
                .Select(x => 
                    new Tuple<Skills, int>(EnumExtensions.GetValueFromDescription<Skills>(x.Item1), x.Item2))
                .ToList();
            
            return new Player(playerIndex,
                skills.Aggregate(Skills.None, (current, skill) => current | skill.Item1),
                SkillSuccess(skills, Skills.Loner),
                SkillSuccess(skills, Skills.Pro));
        }

        private static double? SkillSuccess(IEnumerable<Tuple<Skills, int>> skillsAndRolls, Skills skill)
        {
            var roll = skillsAndRolls.SingleOrDefault(x => x.Item1 == skill)?.Item2;

            return (7.0 - roll) / 6;
        }

        private static Tuple<string, int> GetSkillAndRoll(string input)
        {
            var last = input.Last();

            return last is '2' or '3' or '4' or '5' or '6'
                ? new Tuple<string, int>(input.Remove(input.Length - 1), int.Parse(last.ToString()))
                : input is "L"
	                ? new Tuple<string, int>(input, 4) //assume default loner roll is 4
	                : input is "P" 
                        ? new Tuple<string, int>(input, 3)
                        : new Tuple<string, int>(input, 0);
        }
    }
}
