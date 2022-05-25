using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface IPlayerBuilder
    {
        Player Build(string skillsInput);
    }
}