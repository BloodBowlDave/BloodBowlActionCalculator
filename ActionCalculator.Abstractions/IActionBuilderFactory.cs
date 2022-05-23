namespace ActionCalculator.Abstractions
{
    public interface IActionBuilderFactory
    {
        IActionBuilder GetActionBuilder(string input);
    }
}