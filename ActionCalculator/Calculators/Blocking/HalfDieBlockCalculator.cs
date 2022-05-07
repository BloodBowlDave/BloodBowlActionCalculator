using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Calculators.Blocking
{
    public class HalfDieBlockCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;
        private readonly IBrawlerCalculator _brawlerCalculator;

        public HalfDieBlockCalculator(ICalculator calculator,
            IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
            _brawlerCalculator = brawlerCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            _calculator.Calculate(p * playerAction.Action.Success, r, playerAction, usedSkills);

            CalculateOneDiceFails(p, r, playerAction, usedSkills);
            CalculateTwoDiceFail(p, r, playerAction, usedSkills);
        }

        private void CalculateOneDiceFails(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
        {
            var action = playerAction.Action;
            var player = playerAction.Player;
            var oneDieSuccess = action.SuccessOnOneDie;
            var oneDiceFails = oneDieSuccess * (1 - oneDieSuccess) * 2;
            var canUseBrawler = 0m;

            if (_brawlerCalculator.UseBrawler(r, playerAction))
            {
                canUseBrawler = _brawlerCalculator.ProbabilityCanUseBrawler(action);
                _calculator.Calculate(p * canUseBrawler * oneDieSuccess, r, playerAction, usedSkills);
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills, oneDieSuccess, action.Success))
            {
                _calculator.Calculate(p * (oneDiceFails - canUseBrawler) * player.ProSuccess * oneDieSuccess, r, playerAction, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _calculator.Calculate(p * (oneDiceFails - canUseBrawler) * player.UseReroll * action.Success, r - 1, playerAction, usedSkills);
            }
        }

        private void CalculateTwoDiceFail(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
        {
            var action = playerAction.Action;
            var player = playerAction.Player;
            var oneDieSuccess = action.SuccessOnOneDie;
            var twoFailures = (1 - oneDieSuccess) * (1 - oneDieSuccess);
            var canUseBrawlerAndPro = 0m;

            if (_brawlerCalculator.UseBrawlerAndPro(r, playerAction, usedSkills))
            {
                canUseBrawlerAndPro = _brawlerCalculator.ProbabilityCanUseBrawlerAndPro(action);
                _calculator.Calculate(p * canUseBrawlerAndPro * player.ProSuccess * action.Success, r, playerAction, usedSkills | Skills.Pro);
            }

            if (r > 0)
            {
                _calculator.Calculate(p * (twoFailures - canUseBrawlerAndPro) * player.UseReroll * action.Success, r - 1, playerAction, usedSkills);
            }
        }
    }
}
