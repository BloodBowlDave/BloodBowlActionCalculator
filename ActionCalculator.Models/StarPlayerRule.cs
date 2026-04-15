namespace ActionCalculator.Models
{
    public record StarPlayerRule(StarPlayer StarPlayer, string RuleName, string Description, StarPlayerSkill Skill, IReadOnlyList<string> Skills);
}
