using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators.Fouling
{
    public class FoulStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ITwoD6 _twoD6;

        public FoulStrategy(IActionMediator actionMediator, ITwoD6 twoD6)
        {
            _actionMediator = actionMediator;
            _twoD6 = twoD6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (player, action, i) = playerAction;
            var hasSkill = player.CanUseSkill;
            var useDpOnArmour = 0m;
            var success = action.Success;

            var hasDirtyPlayer = hasSkill(Skills.DirtyPlayer, usedSkills);
            if (hasDirtyPlayer)
            {
                useDpOnArmour = _twoD6.Success(action.OriginalRoll - player.DirtyPlayerValue) - action.Success;
            }

            var doubleOnArmour = hasSkill(Skills.SneakyGit, usedSkills) ? 0 : _twoD6.RollDouble(action.OriginalRoll);
            var doubleOnInjury = _twoD6.RollDouble(2);
            var noDouble = (1 - doubleOnArmour) * (1 - doubleOnInjury);

            Foul(p * success * noDouble, p * success * (1 - noDouble), r, i, usedSkills);
            p *= useDpOnArmour;
            Foul(p * noDouble, p * (1 - noDouble), r, i, usedSkills | Skills.DirtyPlayer);
        }

        private void Foul(decimal successNoDouble, decimal successWithDouble, int r, int i, Skills usedSkills)
        {
            _actionMediator.Resolve(successNoDouble, r, i, usedSkills);
            _actionMediator.Resolve(successWithDouble, r, i, usedSkills, true);
        }
    }
}