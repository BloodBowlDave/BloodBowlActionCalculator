namespace ActionCalculator.Models
{
    public record StarPlayerRule(StarPlayer StarPlayer, StarPlayerSkill Skill, IReadOnlyList<string> Skills, string SkillsInput);
}
