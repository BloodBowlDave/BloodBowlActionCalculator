using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.BallHandling
{
    public class CatchInaccuratePassCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        private const decimal ScatterToTarget = 24m / 512;
        private const decimal ScatterToTargetOrAdjacent = 240m / 512;
        private const decimal ScatterThenBounceToTarget = (ScatterToTargetOrAdjacent - ScatterToTarget) / 8;

        public CatchInaccuratePassCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;

            var roll = action.OriginalRoll + 1 + (player.HasSkill(Skills.DivingCatch) ? 1 : 0);

            var catchSuccess = (7m - roll.ThisOrMinimum(2).ThisOrMaximum(6)) / 6;
            var catchFailure = 1 - catchSuccess;

            CatchScatteredPass(p, r, playerAction, usedSkills, catchSuccess, catchFailure);
            CatchBouncingBall(p, r, playerAction, usedSkills, catchSuccess, catchFailure);
        }

        private void CatchScatteredPass(decimal p, int r, PlayerAction playerAction, Skills usedSkills,
            decimal catchSuccess, decimal catchFailure)
        {
            var player = playerAction.Player;
            var successfulScatter = player.HasSkill(Skills.DivingCatch)
                ? ScatterToTargetOrAdjacent
                : ScatterToTarget;

            CalculateCatch(p, r, playerAction, usedSkills, successfulScatter * catchSuccess,
                successfulScatter * catchFailure * catchSuccess);
        }

        private void CatchBouncingBall(decimal p, int r, PlayerAction playerAction, Skills usedSkills,
            decimal catchSuccess, decimal catchFailure)
        {
            var player = playerAction.Player;

            if (player.HasSkill(Skills.DivingCatch))
            {
                CalculateDivingCatch(p, r, playerAction, usedSkills, catchSuccess, catchFailure);
                return;
            }

            CalculateCatch(p, r, playerAction, usedSkills, ScatterThenBounceToTarget * catchSuccess,
                ScatterThenBounceToTarget * catchFailure * catchSuccess);
        }

        private void CalculateDivingCatch(decimal p, int r, PlayerAction playerAction, Skills usedSkills, decimal catchSuccess, decimal catchFailure)
        {
            var failDivingCatch = catchFailure * catchFailure;

            var player = playerAction.Player;

            if (player.HasSkill(Skills.Catch))
            {
                _calculator.Calculate(p * failDivingCatch * ScatterThenBounceToTarget * (catchFailure * catchSuccess + catchSuccess),
                    r, playerAction, usedSkills);

                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(
                    p * failDivingCatch * player.ProSuccess * ScatterThenBounceToTarget * catchSuccess, r, playerAction,
                    usedSkills | Skills.Pro);

                if (r > 0)
                {
                    _calculator.Calculate(p * failDivingCatch * player.ProSuccess * ScatterThenBounceToTarget * catchFailure * player.UseReroll * catchSuccess,
                        r - 1, playerAction, usedSkills | Skills.Pro);
                }

                return;
            }

            if (r > 0)
            {
                _calculator.Calculate(p * failDivingCatch * player.UseReroll * ScatterThenBounceToTarget * catchSuccess,
                    r - 1, playerAction, usedSkills);

                if (r > 1)
                {
                    _calculator.Calculate(p * failDivingCatch * player.UseReroll * ScatterThenBounceToTarget
                                                     * catchFailure * player.UseReroll * catchSuccess, r - 2, playerAction,
                        usedSkills);
                }

                return;
            }

            _calculator.Calculate(p * catchFailure * ScatterThenBounceToTarget * catchSuccess, r, playerAction,
                usedSkills);
        }

        private void CalculateCatch(decimal p, int r, PlayerAction playerAction, Skills usedSkills,
            decimal successNoReroll, decimal successWithReroll)
        {
            _calculator.Calculate(p * successNoReroll, r, playerAction, usedSkills);

            p *= successWithReroll;

            var player = playerAction.Player;
            if (player.HasSkill(Skills.Catch))
            {
                _calculator.Calculate(p, r, playerAction, usedSkills);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(p * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _calculator.Calculate(p * player.UseReroll, r - 1, playerAction, usedSkills);
            }
        }
    }
}