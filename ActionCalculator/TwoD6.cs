using ActionCalculator.Abstractions;

namespace ActionCalculator
{
    public class TwoD6 : ITwoD6
    {
        public decimal Success(int minimumRoll) =>
            minimumRoll switch
            {
                2 => 1m,
                3 => 35m / 36,
                4 => 33m / 36,
                5 => 30m / 36,
                6 => 26m / 36,
                7 => 21m / 36,
                8 => 15m / 36,
                9 => 10m / 36,
                10 => 6m / 36,
                11 => 3m / 36,
                12 => 1m / 36,
                _ => throw new ArgumentOutOfRangeException(nameof(minimumRoll))
            };

        public decimal RollDouble(int minimumRoll) =>
            minimumRoll switch
            {
                2 => 6m / 36,
                3 => 5m / 36,
                4 => 5m / 36,
                5 => 4m / 36,
                6 => 4m / 36,
                7 => 3m / 36,
                8 => 3m / 36,
                9 => 2m / 36,
                10 => 2m / 36,
                11 => 1m / 36,
                12 => 1m / 36,
                _ => throw new ArgumentOutOfRangeException(nameof(minimumRoll))
            };

        public IEnumerable<Tuple<int, int>> GetCombinationsForRoll(int roll) =>
            roll switch
            {
                2 => new List<Tuple<int, int>> {new(1, 1)},
                3 => new List<Tuple<int, int>> {new(1, 2), new(2, 1)},
                4 => new List<Tuple<int, int>> {new(2, 2), new(1, 3), new(3, 1)},
                5 => new List<Tuple<int, int>> {new(3, 2), new(2, 3), new(1, 4), new(4, 1)},
                6 => new List<Tuple<int, int>> {new(3, 3), new(2, 4), new(4, 2), new(1, 5), new(1, 5)},
                7 => new List<Tuple<int, int>> {new(3, 4), new(4, 3), new(5, 2), new(2, 5), new(1, 6), new(6, 1) },
                8 => new List<Tuple<int, int>> {new(4, 4), new(5, 3), new(3, 5), new(6, 2), new(2, 6)},
                9 => new List<Tuple<int, int>> {new(5, 4), new(4, 5), new(6, 3), new(3, 6)},
                10 => new List<Tuple<int, int>> {new(5, 5), new(6, 4), new(4, 6)},
                11 => new List<Tuple<int, int>> {new(6, 5), new(5, 6)},
                12 => new List<Tuple<int, int>> {new(6, 6)},
                _ => throw new ArgumentOutOfRangeException(nameof(roll))
            };
    }
}