namespace ActionCalculator.Models.Actions
{
	public class Hypnogaze(int roll, bool usePro, bool rerollFailure) : Action(ActionType.Hypnogaze, roll, usePro)
	{
        public bool RerollFailure { get; } = rerollFailure;

        public override string ToString() => $"{(char) ActionType}{Roll}{(!RerollFailure ? "'" : "")}";
	}
}