using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.BallHandling
{
    public class PassStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;

        public PassStrategy(ICalculator calculator, IProHelper proHelper)
        {
            _calculator = calculator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, canUseSkill) = player;
            var pass = (Pass) playerAction.Action;

            var modifier = pass.Modifier;
            var modifiedRoll = pass.Numerator - modifier;

            var successes = (7m - modifiedRoll).ThisOrMinimum(1).ThisOrMaximum(5);
            var failures = (1m - modifier).ThisOrMinimum(1).ThisOrMaximum(5);
            var inaccuratePasses = 6m - successes - failures;

            var success = successes / 6;
            var failure = failures / 6;

            _calculator.Resolve(p * success, r, i, usedSkills);

            var inaccuratePass = inaccuratePasses / 6;
            var rerollInaccuratePass = pass.RerollInaccuratePass;
            var accuratePassAfterFailure = (failure + (rerollInaccuratePass ? inaccuratePass : 0)) * success;
            var inaccuratePassAfterFailure = (failure + (rerollInaccuratePass ? inaccuratePass : 0)) * inaccuratePass;
            var inaccuratePassWithoutReroll = rerollInaccuratePass ? 0m : inaccuratePass;

            if (canUseSkill(Skills.Pass, usedSkills))
            {
                ExecuteReroll(p, r, i, usedSkills, accuratePassAfterFailure, inaccuratePassWithoutReroll + inaccuratePassAfterFailure);
                return;
            }

            if (_proHelper.UsePro(player, pass, r, usedSkills, success, success))
            {
                _calculator.Resolve(inaccuratePassWithoutReroll, r, i, usedSkills, true);

                usedSkills |= Skills.Pro;
                ExecuteReroll(p * proSuccess, r, i, usedSkills | Skills.Pro, accuratePassAfterFailure, inaccuratePassAfterFailure);
                return;
            }

            if (r > 0 && rerollInaccuratePass)
            {
                ExecuteReroll(p * lonerSuccess, r - 1, i, usedSkills | Skills.Pro, accuratePassAfterFailure, inaccuratePassAfterFailure);
                _calculator.Resolve(p * inaccuratePass * (1 - lonerSuccess), r - 1, i, usedSkills, true);
                return;
            }

            _calculator.Resolve(p * inaccuratePass, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal accuratePass, decimal inaccuratePass)
        {
            _calculator.Resolve(p * accuratePass, r, i, usedSkills);
            _calculator.Resolve(p * inaccuratePass, r, i, usedSkills, true);
        }
    }
}