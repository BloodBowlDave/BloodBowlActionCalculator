namespace ActionCalculator.Abstractions.Calculators
{
    public interface ICalculatorFactory
    {
        IActionStrategy GetActionStrategy(Action action, IActionMediator actionMediator, bool nonCriticalFailure);
    }
}