using ActionCalculator.Abstractions;

namespace ActionCalculator.ProbabilityCalculators
{
    public class BaseProbabilityCalculator : IBaseProbabilityCalculator
    {
        private readonly IProbabilityCalculatorFactory _probabilityCalculatorFactory;
        private CalculationContext _context;

        public BaseProbabilityCalculator(IProbabilityCalculatorFactory probabilityCalculatorFactory)
        {
            _probabilityCalculatorFactory = probabilityCalculatorFactory;
        }

        public void Initialise(CalculationContext context)
        {
	        _context = context;
        }

        public void Calculate(decimal p, int r, PlayerAction previousPlayerAction, Skills usedSkills, bool inaccuratePass = false)
        {
            if (p == 0)
            {
                return;
            }

            var i = previousPlayerAction != null ? previousPlayerAction.Index + 1 : 0;

            if (i == _context.Calculation.PlayerActions.Length)
            {
	            _context.Results[_context.MaxRerolls - r] += p;
	            return;
            }
            
            var playerAction = _context.Calculation.PlayerActions[i];
            
            if (previousPlayerAction?.Player.Index != playerAction.Player.Index)
            {
                usedSkills &= Skills.DivingTackle;
            }

            _probabilityCalculatorFactory
                .CreateProbabilityCalculator(playerAction.Action.ActionType, playerAction.Action.NumberOfDice, this, inaccuratePass)
                .Calculate(p, r, playerAction, usedSkills);            
        }
    }
}
