using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class InterferenceStrategy(ICalculator calculator, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool inaccuratePass = false)
        {
            var player = playerAction.Player;
            var interception = playerAction.Action;

            var success = d6.Success(1, inaccuratePass ? interception.Roll - 1 : interception.Roll);
            var failure = 1 - success;

            if (player.CanUseSkill(CalculatorSkills.CloudBurster, usedSkills))
            {
                calculator.Resolve(p * (1 - (1 - failure) * (1 - failure)), r, i, usedSkills, inaccuratePass);
                return;
            }

            calculator.Resolve(p * failure, r, i, usedSkills, inaccuratePass);
        }
    }
}