namespace ActionCalculator.Models
{
    public record StarPlayerRule(string StarPlayer, string RuleName, string Description, StarPlayerSkill Skill, IReadOnlyList<string> Skills);
}
