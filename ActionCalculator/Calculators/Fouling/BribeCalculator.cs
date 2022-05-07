﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Fouling
{
    public class BribeCalculator : ICalculator
    {
        private readonly ICalculator _calculator;

        public BribeCalculator(ICalculator calculator)
        {
            _calculator = calculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var action = playerAction.Action;

            if (nonCriticalFailure)
            {
                _calculator.Calculate(p * action.Success, r, playerAction, usedSkills);
                _calculator.Calculate(p * action.Failure, r, playerAction, usedSkills, true);
                return;
            }

            _calculator.Calculate(p, r, playerAction, usedSkills);
        }
    }
}