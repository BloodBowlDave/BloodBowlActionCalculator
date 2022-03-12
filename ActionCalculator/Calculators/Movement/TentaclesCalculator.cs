using System;
using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Movement
{
    public class TentaclesCalculator : ICalculator
    {
        public TentaclesCalculator(ICalculator baseCalculator, ProCalculator proCalculator)
        {
            throw new NotImplementedException();
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            throw new NotImplementedException();
        }
    }
}