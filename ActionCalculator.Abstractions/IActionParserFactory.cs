namespace ActionCalculator.Abstractions
{
    public interface IActionParserFactory
    {
        IActionParser GetActionParser(string input);
    }
}