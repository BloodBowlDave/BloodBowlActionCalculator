﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.Movement
{
    public class TentaclesStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly IProHelper _proHelper;

        public TentaclesStrategy(ICalculator calculator, IProHelper proHelper)
        {
            _calculator = calculator;
            _proHelper = proHelper;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, _) = player;
            var tentacles = (Tentacles) playerAction.Action;

            var failure = (decimal)(tentacles.Roll + 1).ThisOrMinimum(1).ThisOrMaximum(6) / 6;
            var success = 1 - failure;

            _calculator.Resolve(p * success, r, i, usedSkills);

            p *= failure;

            if (_proHelper.UsePro(player, tentacles, r, usedSkills, success, success))
            {
                ExecuteReroll(p, r, i, usedSkills | Skills.Pro, proSuccess, success, failure);
                return;
            }

            if (r > 0 && tentacles.RerollFailure)
            {
                ExecuteReroll(p, r - 1, i, usedSkills | Skills.Pro, lonerSuccess, success, failure);
                return;
            }

            _calculator.Resolve(p, r, i, usedSkills, true);
        }

        private void ExecuteReroll(decimal p, int r, int i, Skills usedSkills, decimal rerollSuccess, decimal success, decimal failure)
        {
            _calculator.Resolve(p * rerollSuccess * success, r, i, usedSkills);
            _calculator.Resolve(p * (1 - rerollSuccess + rerollSuccess * failure), r, i, usedSkills, true);
        }
    }
}