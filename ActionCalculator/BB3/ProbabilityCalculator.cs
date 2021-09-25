using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using ActionCalculator.Abstractions;
using Action = ActionCalculator.Abstractions.Action;

namespace ActionCalculator.BB3
{
	public class ProbabilityCalculator : IProbabilityCalculator
    {
	    private readonly IReadOnlyDictionary<ActionType, Skills> _skillRerolls;

	    public ProbabilityCalculator()
	    {
		    _skillRerolls = SkillRerolls();
	    }

	    public IEnumerable<decimal> Calculate(Calculation calculation)
	    {
		    var results = new decimal[calculation.PlayerActions.Length + 1];
			var pAllSucceed = PAllSucceed(calculation.PlayerActions);
			var inaccuratePassIndex = GetInaccuratePassIndex(calculation.PlayerActions);

		    foreach (var indexCombination in GetAllIndexCombinations(calculation))
			{
				if (!IsValidCombination(indexCombination, inaccuratePassIndex))
				{
					continue;
				}

				var playerActions = indexCombination
				    .Select(i => calculation.PlayerActions[i]).ToList();

				if (playerActions.Any(x => ActionIsNotRerollable(x.Action)))
				{
					continue;
				}
				
			    var pAllFail = PAllFail(playerActions);
				var rerollsUsed = playerActions.Count;
			    var lonerMultiplier = 1m;

			    foreach (var playerActionGroup in playerActions
				    .GroupBy(x => $"{x.Player.Index}_{x.Action.ActionType}"))
			    {
				    var firstPlayerAction = playerActionGroup.First();
				    var useSkillReroll = UseSkillReroll(firstPlayerAction);

				    if (useSkillReroll)
				    {
					    rerollsUsed--;
				    }
					
				    lonerMultiplier *= LonerMultiplier(firstPlayerAction.Player,
					    playerActionGroup.Count() - (useSkillReroll ? 1 : 0));
			    }
				
			    results[rerollsUsed] += pAllFail * pAllSucceed * lonerMultiplier;
		    }

            return results;
        }

	    private static bool IsValidCombination(ICollection<int> indexCombination, int inaccuratePassIndex) => 
		    inaccuratePassIndex == -1 
		    || indexCombination.Contains(inaccuratePassIndex) 
		    || indexCombination.All(x => x < inaccuratePassIndex);

	    private static IEnumerable<List<int>> GetAllIndexCombinations(Calculation calculation) =>
		    Enumerable.Range(0, calculation.PlayerActions.Length)
			    .ToList().GetAllCombinations();

	    private static decimal PAllFail(IEnumerable<PlayerAction> playerActions) =>
		    playerActions.Aggregate(1m, (p, x)
			    => p * (x.Action.Failure + x.Action.NonCriticalFailure));

	    private bool UseSkillReroll(PlayerAction playerAction) =>
		    _skillRerolls.TryGetValue(playerAction.Action.ActionType, out var skill) 
		    && playerAction.Player.Skills.HasFlag(skill);
		
        private static Dictionary<ActionType, Skills> SkillRerolls() =>
	        new()
	        {
		        {ActionType.PickUp, Skills.SureHands},
		        {ActionType.Rush, Skills.SureFeet},
		        {ActionType.Dodge, Skills.Dodge},
		        {ActionType.Catch, Skills.Catch},
		        {ActionType.Pass, Skills.Pass},
		        {ActionType.InaccuratePass, Skills.Pass}
	        };

        private static bool ActionIsNotRerollable(Action action) =>
            action.ActionType == ActionType.ArmourBreak
            || action.ActionType == ActionType.Foul
            || action.ActionType == ActionType.Injury
            || action.ActionType == ActionType.OtherNonRerollable;

        private static decimal LonerMultiplier(Player player, int rerolls) =>
            player.Skills.HasFlag(Skills.Loner) && player.LonerSuccess != null
                ? (decimal) Math.Pow((double) player.LonerSuccess, rerolls)
                : 1;
        
        private static int GetInaccuratePassIndex(IReadOnlyList<PlayerAction> playerActions)
        {
	        for (var i = 0; i < playerActions.Count; i++)
	        {
		        if (playerActions[i].Action.ActionType == ActionType.InaccuratePass)
		        {
			        return i;
		        }
	        }

	        return -1;
        }

        private static decimal PAllSucceed(IEnumerable<PlayerAction> otherActions) => 
            otherActions.Aggregate(1m, (current, x) => 
	            current * (x.Action.ActionType == ActionType.InaccuratePass 
		            ? x.Action.NonCriticalFailure
		            : x.Action.Success));
    }
}
