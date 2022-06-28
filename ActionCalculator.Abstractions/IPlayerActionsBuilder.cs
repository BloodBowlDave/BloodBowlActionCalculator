using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface IPlayerActionsBuilder
    {
        PlayerActions Build(string playerActionsString);
    }
}