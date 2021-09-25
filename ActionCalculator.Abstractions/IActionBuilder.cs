namespace ActionCalculator.Abstractions
{
    public interface IActionBuilder
    {
        Action Build(string input);
    }
}