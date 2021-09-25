namespace ActionCalculator.Abstractions
{
    public interface IPlayerParser
    { 
        Player Parse(string skillsInput, int playerIndex);
    }
}