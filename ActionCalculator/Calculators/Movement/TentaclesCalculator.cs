using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Movement
{
    public class TentaclesCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly IProCalculator _proCalculator;

        public TentaclesCalculator(ICalculator calculator, IProCalculator proCalculator)
        {
            _calculator = calculator;
            _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var action = playerAction.Action;
            var player = playerAction.Player;

            _calculator.Calculate(p * action.Success, r, playerAction, usedSkills);

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
                _calculator.Calculate(p * action.Failure * player.ProSuccess * action.Success, r, playerAction, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _calculator.Calculate(p * action.Failure * player.LonerSuccess * action.Success, r - 1, playerAction, usedSkills);
            }
        }
    }
}