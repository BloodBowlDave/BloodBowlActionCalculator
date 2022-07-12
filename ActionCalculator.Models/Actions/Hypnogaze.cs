namespace ActionCalculator.Models.Actions
{
	public class Hypnogaze : Action
	{
		public Hypnogaze(int roll, bool usePro, bool rerollFailure) : base(ActionType.Hypnogaze, roll, usePro)
		{
			RerollFailure = rerollFailure;
		}
    
		public bool RerollFailure { get; }

		public override string ToString() => $"{(char) ActionType}{Numerator}{(!RerollFailure ? "'" : "")}";
	}
}