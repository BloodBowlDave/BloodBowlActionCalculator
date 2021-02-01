using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class PlayerParser : IPlayerParser
    {
        public Player Parse(string input)
        {
            return new Player();
        }
    }
}
