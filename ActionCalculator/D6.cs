using ActionCalculator.Utilities;

namespace ActionCalculator
{
    public class D6 : Abstractions.ID6
    {
        private readonly List<List<int>>[] _rolls;
        private static readonly List<int> D6Rolls = new() {6, 5, 4, 3, 2, 1};

        public D6()
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
                    rolls.Add(D6Rolls);
                }

                yield return rolls.GetCombinationsOfLists().ToList();
            }
        }

        public IEnumerable<int> Rolls() => D6Rolls;

        public List<List<int>> Rolls(int numberOfDice) => _rolls[numberOfDice - 1];

        public decimal Success(int numberOfDice, int minimumRoll) => 
            (decimal)Successes(numberOfDice, NormaliseRoll(numberOfDice, minimumRoll)).Count() / _rolls[numberOfDice - 1].Count;
        
        private static int NormaliseRoll(int numberOfDice, int minimumRoll) =>
            numberOfDice switch
            {
                1 => minimumRoll.NormaliseD6(),
                2 => minimumRoll.Normalise2D6(),
                3 => minimumRoll.Normalise3D6(),
                _ => throw new ArgumentOutOfRangeException(nameof(minimumRoll), minimumRoll, null)
            };

        private IEnumerable<List<int>> Successes(int numberOfDice, int minimumRoll, int maximumRoll = 999) => 
            _rolls[numberOfDice - 1].Where(x =>
            {
	            var sum = x.Sum();
                return sum >= minimumRoll && sum <= maximumRoll;
            });

        public decimal RollDouble(int minimumRoll, int maximumRoll)
        {
            minimumRoll = NormaliseRoll(2, minimumRoll);
            
            return (decimal)Successes(2, minimumRoll, maximumRoll).Count(x => 
                x.Sum() >= minimumRoll && x.Sum() <= maximumRoll && x.All(y => y == x.First())) / _rolls[1].Count;
        }
    }
}