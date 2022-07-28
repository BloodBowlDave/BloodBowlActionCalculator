using FluentValidation.Results;

namespace ActionCalculator.Models
{
    public class CalculationResult
    {
        public CalculationResult(decimal[] results)
        {
            Results = results;
            IsValid = true;
            ValidationFailures = new List<ValidationFailure>();
        }

        public CalculationResult(ICollection<ValidationFailure> validationFailures)
        {
            Results = Array.Empty<decimal>();
            IsValid = false;
            ValidationFailures = validationFailures;
        }

        public decimal[] Results { get; }
        public bool IsValid { get; }
        public ICollection<ValidationFailure> ValidationFailures { get; }
    }
}