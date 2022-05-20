namespace ActionCalculator.Abstractions.Calculators.Blocking;

public interface IRollOutcomeHelper
{
    Dictionary<Tuple<bool, int, Skills>, decimal> GetRollOutcomes(int r, PlayerAction playerAction, Skills usedSkills);
    Dictionary<Tuple<bool, int, Skills>, decimal> GetRollOutcomesForRedDice(int r, PlayerAction playerAction, Skills usedSkills);
}