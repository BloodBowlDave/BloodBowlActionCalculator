namespace ActionCalculator.Models.Actions
{
	public class Hypnogaze : Action
	{
		public Hypnogaze(decimal success, decimal failure, int roll, bool usePro, bool rerollFailure) 
			: base(ActionType.Hypnogaze, success, failure, roll, usePro)
		{
			RerollFailure = rerollFailure;
		}
    
		public bool RerollFailure { get; }

		public override string ToString() => $"{(char) ActionType}{Roll}{(!RerollFailure ? "'" : "")}";
	}
}