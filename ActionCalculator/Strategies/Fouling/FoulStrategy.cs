using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulStrategy(ICalculator calculator, ID6 d6) : IActionStrategy
    {
        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var canUseSkill = player.CanUseSkill;
            var sneakyPairModifier = canUseSkill(CalculatorSkills.ASneakyPair, usedSkills) ? 1 : 0;
            var roll = playerAction.Action.Roll - sneakyPairModifier;
            var baseSuccess = d6.Success(2, roll);
            var failure = 1 - baseSuccess;

            var canUseSneakyGit = canUseSkill(CalculatorSkills.SneakyGit, usedSkills);
            var rollDouble = canUseSneakyGit ? 0 : d6.RollDouble(roll, 12);
            var canUseLoneFouler = canUseSkill(CalculatorSkills.LoneFouler, usedSkills);

            calculator.Resolve(p * (baseSuccess - rollDouble), r, i, usedSkills);
            calculator.Resolve(p * rollDouble, r, i, usedSkills, true);

            if (!canUseSkill(CalculatorSkills.DirtyPlayer, usedSkills))
            {
                if (canUseLoneFouler)
                {
                    calculator.Resolve(p * failure * (baseSuccess - rollDouble), r, i, usedSkills | CalculatorSkills.LoneFouler);
                    calculator.Resolve(p * failure * rollDouble, r, i, usedSkills | CalculatorSkills.LoneFouler, true);
                }
                return;
            }

            var dpAdditionalSuccess = d6.Success(2, roll - player.DirtyPlayerValue) - baseSuccess;
            var rollDoubleAndUseDirtyPlayer = canUseSneakyGit ? 0 : d6.RollDouble(roll - player.DirtyPlayerValue, roll - 1);

            calculator.Resolve(p * (dpAdditionalSuccess - rollDoubleAndUseDirtyPlayer), r, i, usedSkills | CalculatorSkills.DirtyPlayer);
            calculator.Resolve(p * rollDoubleAndUseDirtyPlayer, r, i, usedSkills | CalculatorSkills.DirtyPlayer, true);

            if (!canUseLoneFouler) return;

            var dpFailure = failure - dpAdditionalSuccess;
            calculator.Resolve(p * dpFailure * (baseSuccess - rollDouble), r, i, usedSkills | CalculatorSkills.LoneFouler);
            calculator.Resolve(p * dpFailure * rollDouble, r, i, usedSkills | CalculatorSkills.LoneFouler, true);
            calculator.Resolve(p * dpFailure * (dpAdditionalSuccess - rollDoubleAndUseDirtyPlayer), r, i, usedSkills | CalculatorSkills.LoneFouler | CalculatorSkills.DirtyPlayer);
            calculator.Resolve(p * dpFailure * rollDoubleAndUseDirtyPlayer, r, i, usedSkills | CalculatorSkills.LoneFouler | CalculatorSkills.DirtyPlayer, true);
        }
    }
}
