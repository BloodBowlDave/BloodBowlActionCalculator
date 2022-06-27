using ActionCalculator.Models;
using Action = ActionCalculator.Models.Actions;

namespace ActionCalculator.Abstractions.Strategies
{
    public interface IProHelper
    {
        bool UsePro(Player player, Action.Action action, int r, Skills usedSkills, decimal successWithPro, decimal successWithReroll);
    }
}