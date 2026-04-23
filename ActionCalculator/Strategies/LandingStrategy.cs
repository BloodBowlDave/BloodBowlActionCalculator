using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies
{
    public class LandingStrategy(ICalculator calculator, IProHelper proHelper, IConsummateProfessionalHelper consummateProfessionalHelper, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var landing = playerAction.Action;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var roll = nonCriticalFailure ? landing.Roll + 1 : landing.Roll;
            var success = d6.Success(1, roll);
            var failure = 1 - success;

            calculator.Resolve(p * success, r, i, usedSkills);

            var consummateProfessionalSuccess = consummateProfessionalHelper.GetAgilityRollSuccess(usedSkills, landing.Roll, canUseSkill);

            if (consummateProfessionalSuccess > 0)
            {
                calculator.Resolve(p * consummateProfessionalSuccess, r, i, usedSkills | CalculatorSkills.Pro);
            }
            failure -= consummateProfessionalSuccess;

            if (proHelper.UsePro(player, landing, r, usedSkills, success, success))
            {
                calculator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | CalculatorSkills.Pro);
                return;
            }

            calculator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills);
        }
    }
}
