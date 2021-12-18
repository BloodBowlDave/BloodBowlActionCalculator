using ActionCalculator.Abstractions;

namespace ActionCalculator.ProbabilityCalculators
{
    public class RerollableActionCalculator : IProbabilityCalculator
    {
        private readonly IProbabilityCalculator _probabilityCalculator;
        private readonly IProCalculator _proCalculator;

        public RerollableActionCalculator(IProbabilityCalculator probabilityCalculator, IProCalculator proCalculator)
        {
	        _probabilityCalculator = probabilityCalculator;
	        _proCalculator = proCalculator;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool inaccuratePass = false)
        {
            var player = playerAction.Player;
            var success = playerAction.Action.Success;
            var failure = playerAction.Action.Failure;

            _probabilityCalculator.Calculate(p * success, r, playerAction, usedSkills);

            p *= failure * success;

            if (_proCalculator.UsePro(playerAction, r, usedSkills))
            {
	            _probabilityCalculator.Calculate(p * player.ProSuccess, r, playerAction, usedSkills | Skills.Pro);
	            return;
            }

            if (r > 0)
            {
	            _probabilityCalculator.Calculate(p * player.LonerSuccess, r - 1, playerAction, usedSkills);
            }
        }
    }
}