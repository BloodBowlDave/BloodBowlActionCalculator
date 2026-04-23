using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class CatchStrategy(ICalculator calculator, IProHelper proHelper, IConsummateProfessionalHelper consummateProfessionalHelper, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var action = playerAction.Action;
            var success = d6.Success(1, action.Roll);
            var failure = 1 - success;

            calculator.Resolve(p * success, r, i, usedSkills);

            var consummateProfessionalSuccess = consummateProfessionalHelper.GetAgilityRollSuccess(usedSkills, action.Roll, canUseSkill);

            if (consummateProfessionalSuccess > 0)
            {
                calculator.Resolve(p * consummateProfessionalSuccess, r, i, usedSkills | CalculatorSkills.Pro);
            }
            failure -= consummateProfessionalSuccess;

            p *= failure * success;

            if (canUseSkill(CalculatorSkills.Catch, usedSkills))
            {
                calculator.Resolve(p, r, i, usedSkills);
                return;
            }

            if (proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                calculator.Resolve(p * proSuccess, r, i, usedSkills | CalculatorSkills.Pro);
                return;
            }

            calculator.Resolve(p * lonerSuccess, r - 1, i, usedSkills);
        }
    }
}
