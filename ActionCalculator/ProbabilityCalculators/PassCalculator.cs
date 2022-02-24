using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.ProbabilityCalculators;

namespace ActionCalculator.ProbabilityCalculators
{
    public class PassCalculator : IProbabilityCalculator
    {
        private readonly IProbabilityCalculator _probabilityCalculator;
        private readonly IProCalculator _proCalculator;

        public PassCalculator(IProbabilityCalculator probabilityCalculator, IProCalculator proCalculator)
        {
	        _probabilityCalculator = probabilityCalculator;
	        _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;

            _probabilityCalculator.Calculate(p * action.Success, r, playerAction, usedSkills);

            var accuratePassAfterFailure = p * (action.Failure + action.NonCriticalFailure) * action.Success;
            var inaccuratePassAfterFailure = p * (action.Failure + action.NonCriticalFailure) * action.NonCriticalFailure;

            if (player.HasSkill(Skills.Pass))
            {
                _probabilityCalculator.Calculate(accuratePassAfterFailure, r, playerAction, usedSkills);
                _probabilityCalculator.Calculate(inaccuratePassAfterFailure, r, playerAction, usedSkills, true);
                return;
            }

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
	            _probabilityCalculator.Calculate(player.ProSuccess * accuratePassAfterFailure, r, playerAction, usedSkills | Skills.Pro);
	            _probabilityCalculator.Calculate(player.ProSuccess * inaccuratePassAfterFailure, r, playerAction, usedSkills | Skills.Pro, true);
                return;
            }

            if (r > 0)
            {
	            _probabilityCalculator.Calculate(player.LonerSuccess * accuratePassAfterFailure, r - 1, playerAction, usedSkills);
	            _probabilityCalculator.Calculate(player.LonerSuccess * inaccuratePassAfterFailure, r - 1, playerAction, usedSkills, true);
	            return;
            }

            _probabilityCalculator.Calculate(p * action.NonCriticalFailure, r, playerAction, usedSkills, true);
        }
    }
}