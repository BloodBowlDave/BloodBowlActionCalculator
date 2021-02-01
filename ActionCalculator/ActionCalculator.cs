using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
    public class ActionCalculator : IActionCalculator
    {
        private List<List<int>> _indexCombinations;
        
        public decimal[] Calculate(Calculation calculation)
        {
            var result = new decimal[calculation.Players.Sum(x => x.Actions.Length) + 1];

            var playerResults = new decimal[calculation.Players.Length][];

            for (var i = 0; i < calculation.Players.Length; i++)
            {
                playerResults[i] = Calculate(calculation.Players[i]);
            }

            return playerResults[0];
        }

        private decimal[] Calculate(Player player)
        {
            var actionCount = player.Actions.Length;
            _indexCombinations = GetAllCombinations(Enumerable.Range(0, actionCount).ToList());
            
            var result = new decimal[actionCount + 1];
            result[0] = PNoRerolls(player.Actions);
            
            for (var rerolls = 1; rerolls <= actionCount; rerolls++)
            {
                result[rerolls] = result[rerolls - 1] + result[0] * PSpecifiedRerolls(player.Actions, rerolls);
            }

            return result;
        }

        private decimal PSpecifiedRerolls(IReadOnlyList<Action> actions, int specifiedRerolls)
        {
            decimal sum = 0;
            foreach (var indexCombination in _indexCombinations)
            {
                if (indexCombination.Count != specifiedRerolls)
                {
                    continue;
                }

                var result = 1m;

                foreach (var index in indexCombination)
                {
                    result *= actions[index].Failure;
                }

                sum += result;
            }

            return sum;
        }

        private static decimal PNoRerolls(IEnumerable<Action> actions) => 
            actions.Aggregate(1m, (current, action) => current * action.Success);

        public static List<List<int>> GetAllCombinations(List<int> list)
        {
            var comboCount = (int)Math.Pow(2, list.Count) - 1;
            var result = new List<List<int>>();

            for (var i = 1; i < comboCount + 1; i++)
            {
                result.Add(new List<int>());

                for (var j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                    {
                        result.Last().Add(list[j]);
                    }
                }
            }

            return result;
        }
    }
}
