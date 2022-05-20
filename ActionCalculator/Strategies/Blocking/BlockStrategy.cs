using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;
using ActionCalculator.Abstractions.Calculators.Blocking;

namespace ActionCalculator.Strategies.Blocking
{
    public class BlockStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly IRollOutcomeHelper _rollOutcomeHelper;

        public BlockStrategy(IActionMediator actionMediator, IRollOutcomeHelper rollOutcomeHelper)
        {
            _actionMediator = actionMediator;
            _rollOutcomeHelper = rollOutcomeHelper;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var rollOutcomes = _rollOutcomeHelper.GetRollOutcomes(r, playerAction, usedSkills);
            var i = playerAction.Index;
            
            foreach (var rollOutcome in rollOutcomes)
            {
                var ((asdas, rerollsRemaining, usedSkillsForRollOutcome), hello) = rollOutcome;
                
                _actionMediator.Resolve(p * hello, rerollsRemaining, i, usedSkillsForRollOutcome, asdas);
            }
        }
    }
}
