using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Movement
{
    public class LeapStrategy(ICalculator calculator, IProHelper proHelper, IConsummateProfessionalHelper consummateProfessionalHelper, ID6 d6) : IActionStrategy
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

            var consummateProfessionalSuccess = consummateProfessionalHelper.GetAgilityRollSuccess(usedSkills, action.Roll + (useDivingTackle ? 2 : 0), canUseSkill);

            if (consummateProfessionalSuccess > 0)
            {
                calculator.Resolve(p * consummateProfessionalSuccess, r, i, usedSkills | CalculatorSkills.Pro);
            }

            failure -= consummateProfessionalSuccess;

            var pSteadyFooting = canUseSkill(CalculatorSkills.SteadyFooting, usedSkills) ? 1m / 6 : 0;

            if (canUseSkill(CalculatorSkills.BoundingLeap, usedSkills))
            {
                usedSkills |= CalculatorSkills.BoundingLeap;

                calculator.Resolve(p * failure * success, r, i, usedSkills);
                calculator.Resolve(p * failureWithDivingTackle * success, r, i, usedSkills | CalculatorSkills.DivingTackle);
                ResolveSteadyFooting(p * failure * (1m - success), r, i, usedSkills, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * failureWithDivingTackle * (1m - success), r, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
                return;
            }

            if (proHelper.UsePro(player, action, r, usedSkills, success, success))
            {
                usedSkills |= CalculatorSkills.Pro;

                calculator.Resolve(p * proSuccess * failure * success, r, i, usedSkills);
                calculator.Resolve(p * proSuccess * failureWithDivingTackle * success, r, i, usedSkills | CalculatorSkills.DivingTackle);
                ResolveSteadyFooting(p * proSuccess * failure * (1m - success), r, i, usedSkills, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * proSuccess * failureWithDivingTackle * (1m - success), r, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * (1m - proSuccess) * failure, r, i, usedSkills, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * (1m - proSuccess) * failureWithDivingTackle, r, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
                return;
            }

            if (r == 0)
            {
                ResolveSteadyFooting(p * failure, r, i, usedSkills, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * failureWithDivingTackle, r, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
            }
            else
            {
                calculator.Resolve(p * lonerSuccess * failure * success, r - 1, i, usedSkills);
                calculator.Resolve(p * lonerSuccess * failureWithDivingTackle * success, r - 1, i, usedSkills | CalculatorSkills.DivingTackle);
                ResolveSteadyFooting(p * lonerSuccess * failure * (1m - success), r - 1, i, usedSkills, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * lonerSuccess * failureWithDivingTackle * (1m - success), r - 1, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * (1m - lonerSuccess) * failure, r - 1, i, usedSkills, pSteadyFooting, lonerSuccess);
                ResolveSteadyFooting(p * (1m - lonerSuccess) * failureWithDivingTackle, r - 1, i, usedSkills | CalculatorSkills.DivingTackle, pSteadyFooting, lonerSuccess);
            }
        }

        private void ResolveSteadyFooting(decimal p, int r, int i, CalculatorSkills usedSkills, decimal pSteadyFooting, decimal lonerSuccess)
        {
            if (pSteadyFooting == 0) return;
            calculator.Resolve(p * pSteadyFooting, r, i, usedSkills | CalculatorSkills.SteadyFooting);
            calculator.Resolve(p * (1m - pSteadyFooting) * lonerSuccess * pSteadyFooting, r - 1, i, usedSkills | CalculatorSkills.SteadyFooting);
        }
    }
}
