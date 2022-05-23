namespace ActionCalculator.Abstractions
{
    public interface IActionBuilder
    {
        public Action Build(string input);
    }
}
