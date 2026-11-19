namespace ActionCalculator
{
    public class CalculationParseException(string message, Exception? innerException = null) : Exception(message, innerException);
}
