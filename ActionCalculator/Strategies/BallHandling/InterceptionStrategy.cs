using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies.BallHandling
{
    public class InterceptionStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;

        public InterceptionStrategy(IActionMediator actionMediator)
        {
            _actionMediator = actionMediator;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (player, action, i) = playerAction;

            var interceptionFailure = nonCriticalFailure
                ? 1 - (7m - (action.OriginalRoll - 1).ThisOrMinimum(2).ThisOrMaximum(6)) / 6
                : action.Failure;

            if (player.CanUseSkill(Skills.CloudBurster, usedSkills))
            {
                _actionMediator.Resolve(p * (1 - (1 - interceptionFailure) * (1 - interceptionFailure)), r, i, usedSkills, nonCriticalFailure);
                return;
            }

            _actionMediator.Resolve(p * interceptionFailure, r, i, usedSkills, nonCriticalFailure);
        }
    }
}