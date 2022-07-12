using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class InterferenceStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _d6;

        public InterferenceStrategy(IActionMediator actionMediator, ID6 d6)
        {
            _actionMediator = actionMediator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool inaccuratePass = false)
        {
            var player = playerAction.Player;
            var interception = playerAction.Action;
            var i = playerAction.Index;

            var success = _d6.Success(1, inaccuratePass ? interception.Numerator - 1 : interception.Numerator);
            var failure = 1 - success;

            if (player.CanUseSkill(Skills.CloudBurster, usedSkills))
            {
                _actionMediator.Resolve(p * (1 - (1 - failure) * (1 - failure)), r, i, usedSkills, inaccuratePass);
                return;
            }

            _actionMediator.Resolve(p * failure, r, i, usedSkills, inaccuratePass);
        }
    }
}