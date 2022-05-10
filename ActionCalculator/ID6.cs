using ActionCalculator.Utilities;

namespace ActionCalculator
{
    public class ID6 : Abstractions.ID6
    {
        private readonly List<List<int>>[] _rolls;

        public ID6()
        {
            _rolls = GenerateRolls().ToArray();
        }

        private static IEnumerable<List<List<int>>> GenerateRolls()
        {
            for (var i = 1; i <= 3; i++)
            {
                var rolls = new List<List<int>>();

                for (var j = 0; j < i; j++)
                {
                    rolls.Add(new List<int> { 1, 2, 3, 4, 5, 6 });
                }

                yield return rolls.GetCombinationsOfLists().ToList();
            }
        }

        public List<List<int>> Rolls(int numberOfDice) => _rolls[numberOfDice - 1];

        public decimal Success(int numberOfDice, int minimumRoll) => 
            (decimal)Successes(numberOfDice, minimumRoll).Count() / _rolls[numberOfDice - 1].Count;

        private IEnumerable<List<int>> Successes(int numberOfDice, int minimumRoll) => 
            _rolls[numberOfDice - 1].Where(x => x.Sum() >= minimumRoll);

        public decimal RollDouble(int numberOfDice, int minimumRoll) => 
            (decimal)Successes(numberOfDice, minimumRoll)
                .Count(x => x.Sum() >= minimumRoll && x.All(y => y == x.First())) / _rolls[numberOfDice - 1].Count;
    }
}