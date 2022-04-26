namespace ActionCalculator.Abstractions.Calculators
{
    public interface ICalculatorFactory
    {
        ICalculator CreateProbabilityCalculator(Action action, ICalculator calculator, bool nonCriticalFailure);
    }
}