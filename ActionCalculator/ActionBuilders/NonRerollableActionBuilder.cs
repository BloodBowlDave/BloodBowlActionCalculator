using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class NonRerollableActionBuilder : IActionBuilder
{
    private readonly ID6 _d6;

    public NonRerollableActionBuilder(ID6 d6)
    {
        _d6 = d6;
    }

    public Action Build(string input)
    {
        if (input.Contains("/"))
        {
            var split = input.Split('/');
            var numerator = int.Parse(split[0][1..]);
            var denominator = int.Parse(split[1]);

            if (denominator == 12)
            {
                var twoD6Success = _d6.Success(2, numerator);

                return new NonRerollableAction(twoD6Success, 1 - twoD6Success, numerator, denominator);
            }

            var success = (decimal)numerator / denominator;
            
            return new NonRerollableAction(success, 1 - success, numerator, denominator);
        }
        else
        {
            var roll = int.Parse(input.Length == 2 ? input[1..] : input);
            var success = _d6.Success(1, roll);

            return new NonRerollableAction(success, 1 - success, roll, 6);
        }
    }
}