using ActionCalculator.Abstractions;
using ActionCalculator.Abstractions.Calculators;

namespace ActionCalculator.Calculators
{
    public class ArmourBreakCalculator : ICalculator
    {
        private readonly ICalculator _calculator;
        private readonly ITwoD6 _twoD6;

        public ArmourBreakCalculator(ICalculator calculator, ITwoD6 twoD6)
        {
            _calculator = calculator;
            _twoD6 = twoD6;
        }

        public void Calculate(decimal p, int r, PlayerAction playerAction, Skills usedSkills, bool nonCriticalFailure = false)
        {
            var player = playerAction.Player;
            var action = playerAction.Action;

            var armourRoll = action.OriginalRoll;
            if (player.HasSkill(Skills.Claw) && armourRoll >= 8)
            {
                _calculator.Calculate(p * _twoD6.Success(8), r, playerAction, usedSkills);
                return;
            }

            var success = action.Success;
            var succeedUsingPreviousSkills = action.Success;
            var useCrushingBlow = 0m;
            var hasCrushingBlow = player.HasSkill(Skills.CrushingBlow);
            if (hasCrushingBlow)
            {
                useCrushingBlow = _twoD6.Success(armourRoll - 1) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useCrushingBlow;
            }

            var useRam = 0m;
            var hasRam = player.HasSkill(Skills.Ram);
            if (hasRam)
            {
                useRam = _twoD6.Success(armourRoll - 1) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useRam;
            }

            var useSlayer = 0m;
            var hasSlayer = player.HasSkill(Skills.Slayer);
            if (hasSlayer)
            {
                useSlayer = _twoD6.Success(armourRoll - 1) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useSlayer;
            }

            var useCrushingBlowAndRam = 0m;
            if (hasCrushingBlow && hasRam)
            {
                useCrushingBlowAndRam = _twoD6.Success(armourRoll - 2) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useCrushingBlowAndRam;
            }

            var useMightyBlow = 0m;
            var hasMightyBlow = player.HasSkill(Skills.MightyBlow);
            var mightyBlowValue = player.MightyBlowValue;
            if (hasMightyBlow)
            {
                useMightyBlow = _twoD6.Success(armourRoll - mightyBlowValue) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useMightyBlow;
            }

            var useCrushingBlowAndSlayer = 0m;
            if (hasCrushingBlow && hasSlayer)
            {
                useCrushingBlowAndSlayer = _twoD6.Success(armourRoll - 2) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useCrushingBlowAndSlayer;
            }

            var useSlayerAndRam = 0m;
            if (hasSlayer && hasRam)
            {
                useSlayerAndRam = _twoD6.Success(armourRoll - 2) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useSlayerAndRam;
            }

            var useSlayerAndMightyBlow = 0m;
            if (hasSlayer && hasRam)
            {
                useSlayerAndMightyBlow = _twoD6.Success(armourRoll - mightyBlowValue - 1) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useSlayerAndMightyBlow;
            }

            var useCrushingBlowAndMightyBlow = 0m;
            if (hasCrushingBlow && hasMightyBlow)
            {
                useCrushingBlowAndMightyBlow = _twoD6.Success(armourRoll - mightyBlowValue - 1) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useCrushingBlowAndMightyBlow;
            }

            var useRamAndMightyBlow = 0m;
            if (hasRam && hasMightyBlow)
            {
                useRamAndMightyBlow = _twoD6.Success(armourRoll - mightyBlowValue - 1) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useRamAndMightyBlow;
            }

            var useCrushingBlowSlayerAndRam = 0m;
            if (hasCrushingBlow && hasSlayer && hasRam)
            {
                useCrushingBlowSlayerAndRam = _twoD6.Success(armourRoll - 3) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useCrushingBlowSlayerAndRam;
            }

            var useCrushingBlowRamAndMightyBlow = 0m;
            if (hasCrushingBlow && hasRam && hasMightyBlow)
            {
                useCrushingBlowRamAndMightyBlow = _twoD6.Success(armourRoll - mightyBlowValue - 2) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useCrushingBlowRamAndMightyBlow;
            }

            var useCrushingBlowSlayerAndMightyBlow = 0m;
            if (hasCrushingBlow && hasSlayer && hasMightyBlow)
            {
                useCrushingBlowSlayerAndMightyBlow = _twoD6.Success(armourRoll - mightyBlowValue - 2) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useCrushingBlowSlayerAndMightyBlow;
            }

            var useMightyBlowSlayerAndRam = 0m;
            if (hasMightyBlow && hasSlayer && hasRam)
            {
                useMightyBlowSlayerAndRam = _twoD6.Success(armourRoll - mightyBlowValue - 2) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useMightyBlowSlayerAndRam;
            }

            var useCrushingBlowRamSlayerAndMightyBlow = 0m;
            if (hasCrushingBlow && hasSlayer && hasRam && hasMightyBlow)
            {
                useCrushingBlowRamSlayerAndMightyBlow = _twoD6.Success(armourRoll - mightyBlowValue - 3) - succeedUsingPreviousSkills;
                succeedUsingPreviousSkills += useCrushingBlowRamSlayerAndMightyBlow;
            }

            _calculator.Calculate(p * (success + useCrushingBlow), r, playerAction, usedSkills);
            _calculator.Calculate(p * (useMightyBlow + useCrushingBlowAndMightyBlow), r, playerAction, usedSkills | Skills.MightyBlow);
            _calculator.Calculate(p * (useRam + useCrushingBlowAndRam), r, playerAction, usedSkills | Skills.Ram);
            _calculator.Calculate(p * (useSlayer + useCrushingBlowAndSlayer), r, playerAction, usedSkills | Skills.Slayer);
            _calculator.Calculate(p * (useSlayerAndMightyBlow + useCrushingBlowSlayerAndMightyBlow), r, playerAction, usedSkills | Skills.Slayer | Skills.MightyBlow);
            _calculator.Calculate(p * (useRamAndMightyBlow + useCrushingBlowRamAndMightyBlow), r, playerAction, usedSkills | Skills.Ram | Skills.MightyBlow);
            _calculator.Calculate(p * (useSlayerAndRam + useCrushingBlowSlayerAndRam), r, playerAction, usedSkills | Skills.Ram | Skills.Slayer);
            _calculator.Calculate(p * (useMightyBlowSlayerAndRam + useCrushingBlowRamSlayerAndMightyBlow), r, playerAction, usedSkills | Skills.Ram | Skills.Slayer | Skills.MightyBlow);
        }
    }
}