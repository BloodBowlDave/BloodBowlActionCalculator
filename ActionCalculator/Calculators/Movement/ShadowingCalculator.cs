using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Movement
{
    public class ShadowingCalculator : ICalculator
    {
        public ShadowingCalculator(ICalculator _calculator, ProCalculator proCalculator)
        {
            throw new NotImplementedException();
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            throw new NotImplementedException();
        }
    }
}