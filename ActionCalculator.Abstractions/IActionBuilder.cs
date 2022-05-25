namespace ActionCalculator.Abstractions
{
    public interface IActionBuilder
    {
        public Models.Actions.Action Build(string input);
    }
}
