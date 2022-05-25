using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;

namespace ActionCalculator.Strategies
{
    public class LandingStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IProHelper _proHelper;

        public LandingStrategy(IActionMediator actionMediator, IProHelper proHelper)
        {
            _actionMediator = actionMediator;
            _proHelper = proHelper;
        }
        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var (lonerSuccess, proSuccess, _) = player;
            var landing = (Landing) playerAction.Action;
            var i = playerAction.Index;
            var success = nonCriticalFailure ? (7m - (landing.Roll + 1).ThisOrMinimum(2).ThisOrMaximum(6)) / 6 : landing.Success;
            var failure = 1m - success;

            _actionMediator.Resolve(p * success, r, i, usedSkills);

            if (_proHelper.UsePro(player, landing, r, usedSkills, success, success))
            {
                _actionMediator.Resolve(p * failure * proSuccess * success, r, i, usedSkills | Skills.Pro);
                return;
            }
        
            _actionMediator.Resolve(p * failure * lonerSuccess * success, r - 1, i, usedSkills);
        }
    }
}
