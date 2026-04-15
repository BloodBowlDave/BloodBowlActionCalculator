using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _d6;

        public FoulStrategy(ICalculator calculator, ID6 d6)
        {
            _calculator = calculator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, CalculatorSkills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var canUseSkill = player.CanUseSkill;
            var roll = playerAction.Action.Roll;
            var baseSuccess = _d6.Success(2, roll);
            var failure = 1 - baseSuccess;

            var canUseSneakyGit = canUseSkill(CalculatorSkills.SneakyGit, usedSkills);
            var rollDouble = canUseSneakyGit ? 0 : _d6.RollDouble(roll, 12);
            var canUseLoneFouler = canUseSkill(CalculatorSkills.LoneFouler, usedSkills);

            _calculator.Resolve(p * (baseSuccess - rollDouble), r, i, usedSkills);
            _calculator.Resolve(p * rollDouble, r, i, usedSkills, true);

            if (!canUseSkill(CalculatorSkills.DirtyPlayer, usedSkills))
            {
                if (canUseLoneFouler)
                {
                    _calculator.Resolve(p * failure * (baseSuccess - rollDouble), r, i, usedSkills | CalculatorSkills.LoneFouler);
                    _calculator.Resolve(p * failure * rollDouble, r, i, usedSkills | CalculatorSkills.LoneFouler, true);
                }
                return;
            }

            var dpAdditionalSuccess = _d6.Success(2, roll - player.DirtyPlayerValue) - baseSuccess;
            var rollDoubleAndUseDirtyPlayer = canUseSneakyGit ? 0 : _d6.RollDouble(roll - player.DirtyPlayerValue, roll - 1);

            _calculator.Resolve(p * (dpAdditionalSuccess - rollDoubleAndUseDirtyPlayer), r, i, usedSkills | CalculatorSkills.DirtyPlayer);
            _calculator.Resolve(p * rollDoubleAndUseDirtyPlayer, r, i, usedSkills | CalculatorSkills.DirtyPlayer, true);

            if (!canUseLoneFouler) return;

            var dpFailure = failure - dpAdditionalSuccess;
            _calculator.Resolve(p * dpFailure * (baseSuccess - rollDouble), r, i, usedSkills | CalculatorSkills.LoneFouler);
            _calculator.Resolve(p * dpFailure * rollDouble, r, i, usedSkills | CalculatorSkills.LoneFouler, true);
            _calculator.Resolve(p * dpFailure * (dpAdditionalSuccess - rollDoubleAndUseDirtyPlayer), r, i, usedSkills | CalculatorSkills.LoneFouler | CalculatorSkills.DirtyPlayer);
            _calculator.Resolve(p * dpFailure * rollDoubleAndUseDirtyPlayer, r, i, usedSkills | CalculatorSkills.LoneFouler | CalculatorSkills.DirtyPlayer, true);
        }
    }
}
