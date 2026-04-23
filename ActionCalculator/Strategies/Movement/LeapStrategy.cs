using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Movement
{
    public class LeapStrategy(ICalculator calculator, IProHelper proHelper, IConsummateProfessionalHelper consummateProfessionalHelper,ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = (Models.Actions.Leap)playerAction.Action;
            var (lonerSuccess, proSuccess, canUseSkill) = player;

            var useDivingTackle = action.UseDivingTackle && !usedSkills.Contains(CalculatorSkills.DivingTackle);

            var success = d6.Success(1, action.Roll);
            var failure = 1 - success;

            var failureWithDivingTackle = 0m;
            if (useDivingTackle)
            {
                var successWithDivingTackle = d6.Success(1, action.Roll + 2);
                failureWithDivingTackle = success - successWithDivingTackle;
                success = successWithDivingTackle;
            }

            calculator.Resolve(p * success, r, i, usedSkills);

            var consummateProfessionalSuccess = consummateProfessionalHelper.GetAgilityRollSuccess(usedSkills, action.Roll, canUseSkill);

            if (consummateProfessionalSuccess > 0)
            {
                calculator.Resolve(p * consummateProfessionalSuccess, r, i, usedSkills | CalculatorSkills.Pro);
            }

            failure -= consummateProfessionalSuccess;

            if (canUseSkill(CalculatorSkills.BoundingLeap, usedSkills))
            {
                usedSkills |= CalculatorSkills.BoundingLeap;

                calculator.Resolve(p * failure * success, r, i, usedSkills);
                calculator.Resolve(p * failureWithDivingTackle * success, r, i, usedSkills | CalculatorSkills.DivingTackle); 
                return;
            }

            if (proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                usedSkills |= CalculatorSkills.Pro;

                calculator.Resolve(p * proSuccess * failure * success, r, i, usedSkills);
                calculator.Resolve(p * proSuccess * failureWithDivingTackle * success, r, i, usedSkills | CalculatorSkills.DivingTackle);
                return;
            }

            calculator.Resolve(p * lonerSuccess * failure * success, r - 1, i, usedSkills);
            calculator.Resolve(p * lonerSuccess * failureWithDivingTackle * success, r - 1, i, usedSkills | CalculatorSkills.DivingTackle);
        }
    }
}
