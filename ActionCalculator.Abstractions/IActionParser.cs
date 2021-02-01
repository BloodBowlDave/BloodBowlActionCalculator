namespace ActionCalculator.Abstractions
{
    public interface IActionParser
    {
        Action Parse(string input);
    }
}