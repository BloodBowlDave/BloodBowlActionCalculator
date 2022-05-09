﻿using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Calculators.Blocking
{
    public class ThirdDieBlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProCalculator _proCalculator;
        private readonly IBrawlerCalculator _brawlerCalculator;

        public ThirdDieBlockStrategy(IActionMediator actionMediator, IProCalculator proCalculator, IBrawlerCalculator brawlerCalculator)
        {
            _actionMediator = actionMediator;
            _proCalculator = proCalculator;
            _brawlerCalculator = brawlerCalculator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            _actionMediator.Resolve(p * playerAction.Action.Success, r, playerAction.Index, usedSkills);

            OneDiceFails(p, r, playerAction, usedSkills);
            TwoDiceFail(p, r, playerAction, usedSkills);
            ThreeDiceFail(p, r, playerAction, usedSkills);
        }

        private void OneDiceFails(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
        {
            var ((rerollSuccess, proSuccess, _), action, i) = playerAction;
            var oneDieSuccess = action.SuccessOnOneDie;
            var oneDiceFails = oneDieSuccess * oneDieSuccess * (1 - oneDieSuccess) * 3;
            var canUseBrawler = 0m;

            if (_brawlerCalculator.UseBrawler(r, playerAction, usedSkills))
            {
                canUseBrawler = _brawlerCalculator.ProbabilityCanUseBrawler(action);
                _actionMediator.Resolve(p * canUseBrawler * oneDieSuccess, r, i, usedSkills);
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills, oneDieSuccess, action.Success))
            {
                _actionMediator.Resolve(p * (oneDiceFails - canUseBrawler) * proSuccess * oneDieSuccess, r, i, usedSkills | Skills.Pro);
                return;
            }

            if (r > 0)
            {
                _actionMediator.Resolve(p * (oneDiceFails - canUseBrawler) * rerollSuccess * action.Success, r - 1, i, usedSkills);
            }
        }

        private void TwoDiceFail(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
        {
            var ((rerollSuccess, _, _), action, i) = playerAction;
            var oneDieSuccess = action.SuccessOnOneDie;
            var twoFailures = oneDieSuccess * (1 - oneDieSuccess) * (1 - oneDieSuccess) * 3;
            var canUseBrawler = 0m;

            if (_brawlerCalculator.UseBrawlerAndPro(r, playerAction, usedSkills))
            {
                canUseBrawler = _brawlerCalculator.ProbabilityCanUseBrawlerAndPro(action);
                _actionMediator.Resolve(p * canUseBrawler * oneDieSuccess, r, i, usedSkills | Skills.Pro);
            }

            p *= twoFailures - canUseBrawler;

            if (r > 0)
            {
                _actionMediator.Resolve(p * rerollSuccess * action.Success, r - 1, i, usedSkills);
            }
        }

        private void ThreeDiceFail(decimal p, int r, PlayerAction playerAction, Skills usedSkills)
        {
            if (r == 0)
            {
                return;
            }

            var ((rerollSuccess, _, _), action, i) = playerAction;
            var oneDieSuccess = action.SuccessOnOneDie;
            var threeFailures = (1 - oneDieSuccess) * (1 - oneDieSuccess) * (1 - oneDieSuccess);

            _actionMediator.Resolve(p * threeFailures * rerollSuccess * action.Success, r - 1, i, usedSkills);
        }
    }
}
