using ActionCalculator.Abstractions;
using ActionCalculator.Utilities;

namespace ActionCalculator
{
    public class TwoD6 : ITwoD6
    {
        private readonly List<List<int>> _rolls;

        public TwoD6()
        {
            _rolls = GenerateRolls(2);
        }

        private static List<List<int>> GenerateRolls(int numberOfDice)
        {
            var rolls = new List<List<int>>();

            for (var i = 0; i < numberOfDice; i++)
            {
                rolls.Add(new List<int>{1, 2, 3, 4, 5, 6});
            }
            
            return rolls.GetCombinationsOfLists().ToList();
        }

        public IEnumerable<List<int>> Rolls() => _rolls;

        public decimal Success(int minimumRoll) => (decimal)Successes(minimumRoll).Count() / _rolls.Count;

        private IEnumerable<List<int>> Successes(int minimumRoll) => _rolls.Where(x => x.Sum() >= minimumRoll);

        public decimal RollDouble(int minimumRoll) => 
            (decimal)Successes(minimumRoll).Count(x => x.Sum() >= minimumRoll && x.All(y => y == x.First())) / _rolls.Count;
    }
}