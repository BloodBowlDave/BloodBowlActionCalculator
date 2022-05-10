using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly Abstractions.ID6 _iD6;

        public FoulStrategy(IActionMediator actionMediator, Abstractions.ID6 iD6)
        {
            _actionMediator = actionMediator;
            _iD6 = iD6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var (player, action, i) = playerAction;
            var canUseSkill = player.CanUseSkill;
            var useDpOnArmour = 0m;
            var success = action.Success;

            var hasDirtyPlayer = canUseSkill(Skills.DirtyPlayer, usedSkills);
            if (hasDirtyPlayer)
            {
                useDpOnArmour = _iD6.Success(2, action.OriginalRoll - player.DirtyPlayerValue) - action.Success;
            }

            var doubleOnArmour = canUseSkill(Skills.SneakyGit, usedSkills) ? 0 : _iD6.RollDouble(2, action.OriginalRoll);
            var doubleOnInjury = _iD6.RollDouble(2, 2);
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