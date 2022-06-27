using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;

namespace ActionCalculator.Strategies.BallHandling
{
    public class InterceptionStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _d6;

        public InterceptionStrategy(IActionMediator actionMediator, ID6 d6)
        {
            _actionMediator = actionMediator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var interception = (Interception) playerAction.Action;
            var i = playerAction.Index;

            var interceptionFailure = nonCriticalFailure
                ? 1 - _d6.Success(1, interception.Roll - 1)
                : interception.Failure;

            if (player.CanUseSkill(Skills.CloudBurster, usedSkills))
            {
                _actionMediator.Resolve(p * (1 - (1 - interceptionFailure) * (1 - interceptionFailure)), r, i, usedSkills, nonCriticalFailure);
                return;
            }

            _actionMediator.Resolve(p * interceptionFailure, r, i, usedSkills, nonCriticalFailure);
        }
    }
}