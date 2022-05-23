namespace ActionCalculator.Abstractions.Calculators
{
    public interface IProHelper
    {
        bool UsePro(Player player, Action action, int r, Skills usedSkills, decimal successWithPro, decimal successWithReroll);
    }
}