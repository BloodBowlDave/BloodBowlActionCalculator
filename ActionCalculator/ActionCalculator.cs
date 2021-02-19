using System;
using System.Collections.Generic;
using System.Linq;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator
{
    public class ActionCalculator : IActionCalculator
    {
        public decimal[] Calculate(Calculation calculation)
        {
            var result = P(calculation.Players).ToList();

            RemoveBlanks(result);
            AggregateResults(result);
            
            return result.ToArray();
        }

        private static void RemoveBlanks(IList<decimal> result)
        {
            for (var i = result.Count - 1; i >= 0; i--)
            {
                if (result[i] == 0)
                {
                    result.RemoveAt(i);
                }
            }
        }

        private IEnumerable<decimal> P(IReadOnlyList<Player> players)
        {
            var result = new decimal[players.Sum(x => x.Actions.Length) + 1];
            var playerResults = new decimal[players.Count][];

            for (var playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                playerResults[playerIndex] = PPlayer(players[playerIndex]);
            }

            for (var rerolls = 0; rerolls < result.Length; rerolls++)
            {
                result[rerolls] = PAggregate(playerResults, rerolls);
            }

            return result;
        }

        private static void AggregateResults(IList<decimal> result)
        {
            for (var i = 1; i < result.Count; i++)
            {
                result[i] += result[i - 1];
            }
        }

        private static decimal[] PPlayer(Player player)
        {
            var otherActions = new List<Action>();
            var dodges = new List<Action>();
            var rushes = new List<Action>();
            var nonRerollableAction = new List<Action>();

            foreach ( var action in player.Actions)
            {
                if (player.Skills.HasFlag(Skills.Dodge) && action.ActionType == ActionType.Dodge)
                {
                    dodges.Add(action);
                }
                else if (player.Skills.HasFlag(Skills.SureFeet) && action.ActionType == ActionType.Rush)
                {
                    rushes.Add(action);
                }
                else if (ActionIsNotRerollable(player.Skills, action))
                {
                    nonRerollableAction.Add(action);
                }
                else
                {
                    otherActions.Add(action);
                }
            }

            var actionResults = new decimal[4][];

            actionResults[0] = PActions(otherActions);
            actionResults[1] = PActions(dodges);
            actionResults[2] = PActions(rushes);
            actionResults[3] = new []{ PAllSucceed(nonRerollableAction) };

            if (dodges.Any())
            {
                actionResults[1][1] += actionResults[1][0];
                actionResults[1] = actionResults[1].Skip(1).ToArray();
            }

            if (rushes.Any())
            {
                actionResults[2][1] += actionResults[2][0];
                actionResults[2] = actionResults[2].Skip(1).ToArray();
            }

            var result = new decimal[player.Actions.Length + 1];

            for (var rerolls = 0; rerolls < result.Length; rerolls++)
            {
                result[rerolls] = PAggregate(actionResults, rerolls) * LonerMultiplier(player, rerolls);
            }

            return result;
        }

        private static bool ActionIsNotRerollable(Skills skills, Action action) =>
            skills.HasFlag(Skills.SureHands) && action.ActionType == ActionType.PickUp
            || skills.HasFlag(Skills.Catch) && action.ActionType == ActionType.Catch
            || skills.HasFlag(Skills.Pass) && action.ActionType == ActionType.Pass
            || action.ActionType == ActionType.ArmourBreak
            || action.ActionType == ActionType.Foul
            || action.ActionType == ActionType.Injury
            || action.ActionType == ActionType.OtherNonRerollable;

        private static decimal LonerMultiplier(Player player, int rerolls) =>
            player.Skills.HasFlag(Skills.Loner) && player.LonerSuccess != null
                ? (decimal) Math.Pow((double) player.LonerSuccess, rerolls)
                : 1;
        
        private static decimal[] PActions(IReadOnlyList<Action> actions)
        {
            var result = new decimal[actions.Count + 1];

            result[0] = PAllSucceed(actions);

            for (var i = 1; i < result.Length; i++)
            {
                result[i] = result[0] * PSpecifiedRerolls(actions, i);
            }

            return result;
        }

        private static decimal PAllSucceed(IEnumerable<Action> otherActions) => 
            otherActions.Aggregate(1m, (current, action) => current * action.Success);
        
        private static decimal PSpecifiedRerolls(IReadOnlyList<Action> actions, int specifiedRerolls)
        {
            var indexCombinations = GetAllCombinations(Enumerable.Range(0, actions.Count).ToList());
            
            decimal sum = 0;
            foreach (var indexCombination in indexCombinations)
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

        private static List<List<int>> GetAllCombinations(List<int> list)
        {
            var result = new List<List<int>>();

            for (var i = 1; i < (int)Math.Pow(2, list.Count) - 1 + 1; i++)
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

        private static decimal PAggregate(IReadOnlyList<decimal[]> playerResults, int specifiedRerolls)
        {
            var playerIndices = new List<int>();

            foreach (var playerResult in playerResults)
            {
                playerIndices.AddRange(Enumerable.Range(0, playerResult.Length));
            }

            var indicesCombination = GetIndicesWhereSumToTarget(playerIndices, specifiedRerolls, playerResults.Count);
            
            var playerStartIndices = new List<int>();

            for (var i = 0; i < playerIndices.Count; i++)
            {
                if (playerIndices[i] == 0)
                {
                    playerStartIndices.Add(i);
                }
            }

            indicesCombination = playerStartIndices
                .Select((_, i) => i)
                .Aggregate(indicesCombination, (current, index) =>
                    current.Where(x => 
                        x.Count(y => y >= 
                                     playerStartIndices[index]) == playerStartIndices.Count - index));

            var sum = 0m;

            foreach (var indices in indicesCombination)
            {
                var p = 1m;

                for (var i = 0; i < indices.Length; i++)
                {
                    p *= playerResults[i][indices[i] - playerStartIndices[i]];
                }

                sum += p;
            }

            return sum;
        }

        private static IEnumerable<int[]> GetIndicesWhereSumToTarget(IReadOnlyList<int> indices, int target, int indicesCount)
        {
            var output = new List<int[]>();

            var combinations = (int)(Math.Pow(2, indices.Count));

            for (var i = 0; i < combinations; i++)
            {
                var sum = 0;
                var subIndices = new List<int>();

                for (var j = 0; j < indices.Count; j++)
                {
                    if ((i & (1 << j)) >> j != 1 || sum > target)
                    {
                        continue;
                    }

                    sum += indices[j];
                    
                    subIndices.Add(j);
                }

                if (sum == target && subIndices.Count == indicesCount)
                {
                    output.Add(subIndices.ToArray());
                }
            }

            return output;
        }
    }
}
