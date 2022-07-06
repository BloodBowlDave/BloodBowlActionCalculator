namespace ActionCalculator.Abstractions
{
    public interface IActionParser
    {
        public Models.Actions.Action Parse(string input);
    }
}
