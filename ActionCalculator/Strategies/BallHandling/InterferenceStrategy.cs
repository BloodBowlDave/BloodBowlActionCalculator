using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Strategies;
using ActionCalculator.Models;

namespace ActionCalculator.Strategies.BallHandling
{
    public class InterferenceStrategy : IActionStrategy
    {
        private readonly ICalculator _calculator;
        private readonly ID6 _d6;

        public InterferenceStrategy(ICalculator calculator, ID6 d6)
        {
            _calculator = calculator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, int i, PlayerAction playerAction, Skills usedSkills, bool inaccuratePass = false)
        {
            var player = playerAction.Player;
            var interception = playerAction.Action;

            var success = _d6.Success(1, inaccuratePass ? interception.Numerator - 1 : interception.Numerator);
            var failure = 1 - success;

            if (player.CanUseSkill(Skills.CloudBurster, usedSkills))
            {
                _calculator.Resolve(p * (1 - (1 - failure) * (1 - failure)), r, i, usedSkills, inaccuratePass);
                return;
            }

            _calculator.Resolve(p * failure, r, i, usedSkills, inaccuratePass);
        }
    }
}