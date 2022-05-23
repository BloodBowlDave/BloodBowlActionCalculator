using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Actions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Strategies.Fouling
{
    public class FoulStrategy : IActionStrategy
    {
        private readonly IActionMediator _actionMediator;
        private readonly ID6 _d6;

        public FoulStrategy(IActionMediator actionMediator, ID6 d6)
        {
            _actionMediator = actionMediator;
            _d6 = d6;
        }

        public void Execute(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var canUseSkill = player.CanUseSkill;
            var useDpOnArmour = 0m;
            var foul = (Foul) playerAction.Action;
            var success = _d6.Success(2, foul.Roll);
            var i = playerAction.Index;

            var hasDirtyPlayer = canUseSkill(Skills.DirtyPlayer, usedSkills);
            if (hasDirtyPlayer)
            {
                useDpOnArmour = _d6.Success(2, foul.Roll - player.DirtyPlayerValue) - foul.Success;
            }

            var doubleOnArmour = canUseSkill(Skills.SneakyGit, usedSkills) ? 0 : _d6.RollDouble(2, foul.Roll);
            var doubleOnInjury = _d6.RollDouble(2, 2);
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