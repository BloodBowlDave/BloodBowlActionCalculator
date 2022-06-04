using System;
using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace ActionCalculator.Tests
{
    public class BlastItTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BlastItTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Skipping because it takes too long to run.")]
        public void Test()
        {
            var d8 = new List<Scatter>
            {
                new(0, 1),//2
                new(1, 1),//3
                new(1, 0),//4
                new(1, -1),//5
                new(0, -1),//6
                new(-1, -1),//7
                new(-1, 0),//8
                new(-1, 1)//1
            };

            var groupedFirstScatters = d8.GroupBy(x => x.DistanceFromZero()).ToList();

            var firstScatterStrategies = new List<FirstScatterStrategy>();

            foreach (var firstScatter in groupedFirstScatters.Select(x => x.First()))
            {
                firstScatterStrategies.Add(new FirstScatterStrategy(firstScatter, true));
                firstScatterStrategies.Add(new FirstScatterStrategy(firstScatter, false));
            }

            var firstScatterStrategyCombinations = firstScatterStrategies
                .Combinations()
                .Where(x => x.Length == groupedFirstScatters.Count)
                .Where(x => x.Select(y =>
                    y.Scatter).Distinct().Count() == groupedFirstScatters.Count).ToList();

            var secondScatters = (
                from firstScatterGroup in groupedFirstScatters
                from secondScatter in d8
                select firstScatterGroup.First().Add(secondScatter));

            var groupedSecondScatters = secondScatters
                .GroupBy(x => new Tuple<Scatter, decimal>(x.FirstScatter, x.DistanceFromZero())).ToList();

            var secondScatterStrategies = new List<SecondScatterStrategy>();

            foreach (var secondScatter in groupedSecondScatters.Select(x => x.First()))
            {
                secondScatterStrategies.Add(new SecondScatterStrategy(secondScatter, true));
                secondScatterStrategies.Add(new SecondScatterStrategy(secondScatter, false));
            }

            var secondScatterStrategyCombinations = secondScatterStrategies
                .Combinations()
                .Where(x => x.Length == groupedSecondScatters.Count)
                .Where(x => x.Select(y =>
                    y.SecondScatter).Distinct().Count() == groupedSecondScatters.Count);

            var thirdScatters =
                from secondScatterGroup in groupedSecondScatters
                from thirdScatter in d8
                select secondScatterGroup.First().Add(thirdScatter);

            var groupedThirdScatters = thirdScatters
                .GroupBy(x => new Tuple<SecondScatter, decimal>(x.SecondScatter, x.DistanceFromZero())).ToList();

            var firstTwoScattersStrategies =
                from firstScatterStrategyCombination in firstScatterStrategyCombinations
                from secondScatterStrategyCombination in secondScatterStrategyCombinations
                select new FirstTwoScattersStrategy(firstScatterStrategyCombination, secondScatterStrategyCombination);

            var maximumBouncingDistance = new Scatter(1, 1).DistanceFromZero();
            var thirdScatterStrategies = new List<ThirdScatterStrategy>();

            var directScatterFactor = 1.25m;
            var divingCatch = false;

            foreach (var groupedThirdScatter in groupedThirdScatters)
            {
                var thirdScatter = groupedThirdScatter.First();
                var distanceFromZero = thirdScatter.DistanceFromZero();

                decimal pNoReroll;

                if (distanceFromZero == 0 || distanceFromZero <= maximumBouncingDistance && divingCatch)
                {
                    thirdScatterStrategies.Add(new ThirdScatterStrategy(thirdScatter, false));
                    continue;
                }

                if (distanceFromZero <= maximumBouncingDistance)
                {
                    pNoReroll = (decimal)groupedThirdScatters.Count / 64;
                }
                else
                {
                    thirdScatterStrategies.Add(new ThirdScatterStrategy(thirdScatter, true));
                    continue;
                }

                var pReroll = 0m;

                foreach (var reroll in d8)
                {
                    var rerolledThirdScatter = thirdScatter.SecondScatter.Add(reroll);
                    var rerollDistanceFromZero = rerolledThirdScatter.DistanceFromZero();

                    if (rerollDistanceFromZero == 0 || distanceFromZero <= maximumBouncingDistance && divingCatch)
                    {
                        pReroll += directScatterFactor / 64;
                    }
                    else if (rerollDistanceFromZero <= maximumBouncingDistance)
                    {
                        pReroll += 1m / 512;
                    }
                }

                thirdScatterStrategies.Add(pReroll > pNoReroll
                    ? new ThirdScatterStrategy(thirdScatter, true)
                    : new ThirdScatterStrategy(thirdScatter, false));
            }

            var bestOverallStrategy = new OverallStrategy(null!, null!, null!);
            var best = 0m;
            var bestDirectScatter = 0m;
            var bestBounce = 0m;

            foreach (var firstTwoScattersStrategy in firstTwoScattersStrategies)
            {
                var firstScatterOutcomes = new Dictionary<Scatter, decimal>();

                foreach (var groupedFirstScatter in groupedFirstScatters)
                {
                    var firstScatter = groupedFirstScatter.First();
                    var firstScatterCount = groupedFirstScatter.Count();

                    var firstScatterStrategy = firstTwoScattersStrategy.FirstScatterStrategies.Single(x =>
                        x.Scatter == firstScatter);

                    var firstScatterOutcome = (decimal)firstScatterCount / 8;

                    if (firstScatterStrategy.Reroll)
                    {
                        foreach (var firstScatterRerollGroup in groupedFirstScatters)
                        {
                            var reroll = firstScatterRerollGroup.First();
                            var rerollCount = firstScatterRerollGroup.Count();

                            if (firstScatterOutcomes.ContainsKey(reroll))
                            {
                                firstScatterOutcomes[reroll] += firstScatterOutcome * rerollCount / 8;
                            }
                            else
                            {
                                firstScatterOutcomes.Add(reroll, firstScatterOutcome * rerollCount / 8);
                            }
                        }
                    }
                    else
                    {
                        if (firstScatterOutcomes.ContainsKey(firstScatter))
                        {
                            firstScatterOutcomes[firstScatter] += firstScatterOutcome;
                        }
                        else
                        {
                            firstScatterOutcomes.Add(firstScatter, firstScatterOutcome);
                        }
                    }
                }

                var secondScatterOutcomes = new Dictionary<SecondScatter, decimal>();

                foreach (var (scatter, probability) in firstScatterOutcomes)
                {
                    var secondScatterGroups = groupedSecondScatters.Where(x =>
                        x.Key.Item1 == scatter).ToList();

                    foreach (var groupedSecondScatter in secondScatterGroups)
                    {
                        var secondScatter = groupedSecondScatter.First();
                        var secondScatterCount = groupedSecondScatter.Count();
                        var secondScatterStrategy = firstTwoScattersStrategy.SecondScatterStrategies.Single(x =>
                            x.SecondScatter == secondScatter);

                        var secondScatterProbability = (decimal)secondScatterCount / 8;

                        if (secondScatterStrategy.Reroll)
                        {
                            foreach (var rerolledSecondScatterGroup in secondScatterGroups)
                            {
                                var rerolledSecondScatter = rerolledSecondScatterGroup.First();
                                var rerolledSecondScatterCount = rerolledSecondScatterGroup.Count();

                                if (secondScatterOutcomes.ContainsKey(rerolledSecondScatter))
                                {
                                    secondScatterOutcomes[rerolledSecondScatter] += probability * secondScatterProbability * rerolledSecondScatterCount / 8;
                                }
                                else
                                {
                                    secondScatterOutcomes.Add(rerolledSecondScatter, probability * secondScatterProbability * rerolledSecondScatterCount / 8);
                                }
                            }
                        }
                        else
                        {
                            if (secondScatterOutcomes.ContainsKey(secondScatter))
                            {
                                secondScatterOutcomes[secondScatter] += probability * secondScatterProbability;
                            }
                            else
                            {
                                secondScatterOutcomes.Add(secondScatter, probability * secondScatterProbability);
                            }
                        }
                    }
                }

                var overallStrategy = new OverallStrategy(
                    firstTwoScattersStrategy.FirstScatterStrategies,
                    firstTwoScattersStrategy.SecondScatterStrategies,
                    thirdScatterStrategies);

                decimal p;
                var pDirect = 0m;
                var pBounce = 0m;

                foreach (var (secondScatter, probability) in secondScatterOutcomes)
                {
                    foreach (var groupedThirdScatter in groupedThirdScatters.Where(x =>
                                 x.Key.Item1 == secondScatter))
                    {
                        var thirdScatter = groupedThirdScatter.First();
                        var thirdScatterCount = groupedThirdScatter.Count();
                        var thirdScatterStrategy = overallStrategy.ThirdScatterStrategies.Single(x =>
                            x.ThirdScatter == thirdScatter);

                        if (thirdScatterStrategy.Reroll)
                        {
                            foreach (var rerollGroup in groupedThirdScatters.Where(x =>
                                         x.Key.Item1 == secondScatter))
                            {
                                var reroll = rerollGroup.First();
                                var rerollCount = rerollGroup.Count();
                                var rerollDistanceFromZero = reroll.DistanceFromZero();

                                if (rerollDistanceFromZero == 0 || rerollDistanceFromZero <= maximumBouncingDistance && divingCatch)
                                {
                                    pDirect += probability * thirdScatterCount * rerollCount / 64;
                                }
                                else if (rerollDistanceFromZero <= maximumBouncingDistance)
                                {
                                    pBounce += probability * thirdScatterCount * rerollCount / 512;
                                }
                            }
                        }
                        else
                        {
                            var distanceFromZero = thirdScatter.DistanceFromZero();

                            if (distanceFromZero == 0 || distanceFromZero <= maximumBouncingDistance && divingCatch)
                            {
                                pDirect += probability * thirdScatterCount / 8;
                            }
                            else if (distanceFromZero <= maximumBouncingDistance)
                            {
                                pBounce += probability * thirdScatterCount / 64;
                            }
                        }
                    }
                }

                p = pDirect * directScatterFactor + pBounce;

                if (p <= best)
                {
                    continue;
                }

                bestOverallStrategy = overallStrategy;
                best = p;
                bestDirectScatter = pDirect;
                bestBounce = pBounce;
            }

            foreach (var firstScatterStrategy in bestOverallStrategy.FirstScatterStrategies)
            {
                _testOutputHelper.WriteLine(firstScatterStrategy.ToString());
            }


            foreach (var secondScatterStrategy in bestOverallStrategy.SecondScatterStrategies)
            {
                _testOutputHelper.WriteLine(secondScatterStrategy.ToString());
            }

            var thirdScatterGroups = bestOverallStrategy.ThirdScatterStrategies.GroupBy(x =>
                new Tuple<decimal, decimal>(x.ThirdScatter.SecondScatter.DistanceFromZero(), x.ThirdScatter.DistanceFromZero()));
            
            foreach (var thirdScatterStrategyGroup in bestOverallStrategy.ThirdScatterStrategies.GroupBy(x =>
                         new Tuple<decimal, decimal>(x.ThirdScatter.SecondScatter.DistanceFromZero(), x.ThirdScatter.DistanceFromZero())))
            {
                _testOutputHelper.WriteLine(thirdScatterStrategyGroup.First().ToString());
            }
            
            Assert.Equal(0.124267578125m, bestDirectScatter);
            Assert.Equal(0.0721588134765625m, bestBounce);
        }
    }

    public record Scatter(int X, int Y)
    {
        public readonly int X = X;
        public readonly int Y = Y;

        public decimal DistanceFromZero() => (decimal)Math.Sqrt(X * X + Y * Y);

        public SecondScatter Add(Scatter scatter) => new(this, scatter);
    }

    public record SecondScatter(Scatter FirstScatter, Scatter Scatter)
    {
        public readonly Scatter FirstScatter = FirstScatter;
        public readonly Scatter Scatter = Scatter;

        public int X() => FirstScatter.X + Scatter.X;
        public int Y() => FirstScatter.Y + Scatter.Y;

        public decimal DistanceFromZero() => (decimal)Math.Sqrt(X() * X() + Y() * Y());

        public ThirdScatter Add(Scatter scatter) => new(this, scatter);
    }

    public record ThirdScatter(SecondScatter SecondScatter, Scatter Scatter)
    {
        public readonly SecondScatter SecondScatter = SecondScatter;
        public readonly Scatter Scatter = Scatter;

        private int X() => SecondScatter.X() + Scatter.X;
        private int Y() => SecondScatter.Y() + Scatter.Y;

        public decimal DistanceFromZero() => (decimal)Math.Sqrt(X() * X() + Y() * Y());
    }

    public record FirstScatterStrategy(Scatter Scatter, bool Reroll)
    {
        public readonly Scatter Scatter = Scatter;
        public readonly bool Reroll = Reroll;
    }

    public record SecondScatterStrategy(SecondScatter SecondScatter, bool Reroll)
    {
        public readonly SecondScatter SecondScatter = SecondScatter;
        public readonly bool Reroll = Reroll;
    }

    public record ThirdScatterStrategy(ThirdScatter ThirdScatter, bool Reroll)
    {
        public readonly ThirdScatter ThirdScatter = ThirdScatter;
        public readonly bool Reroll = Reroll;
    }

    public record FirstTwoScattersStrategy(FirstScatterStrategy[] FirstScatterStrategies
        , SecondScatterStrategy[] SecondScatterStrategies)
    {
        public readonly FirstScatterStrategy[] FirstScatterStrategies = FirstScatterStrategies;
        public readonly SecondScatterStrategy[] SecondScatterStrategies = SecondScatterStrategies;
    }

    public record OverallStrategy(FirstScatterStrategy[] FirstScatterStrategies
        , SecondScatterStrategy[] SecondScatterStrategies
        , List<ThirdScatterStrategy> ThirdScatterStrategies)
    {
        public readonly FirstScatterStrategy[] FirstScatterStrategies = FirstScatterStrategies;
        public readonly SecondScatterStrategy[] SecondScatterStrategies = SecondScatterStrategies;
        public readonly List<ThirdScatterStrategy> ThirdScatterStrategies = ThirdScatterStrategies;
    }
}

