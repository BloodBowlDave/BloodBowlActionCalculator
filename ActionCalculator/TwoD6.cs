using System;
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
    }
}