namespace ActionCalculator
{
    public class FailureIndex
    {
        public FailureIndex(int index, bool nonCriticalFailure)
        {
            Index = index;
            NonCriticalFailure = nonCriticalFailure;
        }

        public int Index { get; }
        public bool NonCriticalFailure { get; }
    }
}