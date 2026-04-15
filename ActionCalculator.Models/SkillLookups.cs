namespace ActionCalculator.Models
{
    public static class SkillLookups
    {
        private static readonly IReadOnlyDictionary<AllSkills, SkillCategory> _categoryMap =
            new Dictionary<AllSkills, SkillCategory>
            {
                // Agility
                { AllSkills.Catch,           SkillCategory.Agility },
                { AllSkills.Defensive,       SkillCategory.Agility },
                { AllSkills.DivingCatch,     SkillCategory.Agility },
                { AllSkills.DivingTackle,    SkillCategory.Agility },
                { AllSkills.Dodge,           SkillCategory.Agility },
                { AllSkills.HitAndRun,       SkillCategory.Agility },
                { AllSkills.JumpUp,          SkillCategory.Agility },
                { AllSkills.Leap,            SkillCategory.Agility },
                { AllSkills.SafePairOfHands, SkillCategory.Agility },
                { AllSkills.Sidestep,        SkillCategory.Agility },
                { AllSkills.Sprint,          SkillCategory.Agility },
                { AllSkills.SureFeet,        SkillCategory.Agility },

                // Devious
                { AllSkills.DirtyPlayer,      SkillCategory.Devious },
                { AllSkills.EyeGouge,         SkillCategory.Devious },
                { AllSkills.Fumblerooski,     SkillCategory.Devious },
                { AllSkills.LethalFlight,     SkillCategory.Devious },
                { AllSkills.LoneFouler,       SkillCategory.Devious },
                { AllSkills.PileDriver,       SkillCategory.Devious },
                { AllSkills.PutTheBootIn,     SkillCategory.Devious },
                { AllSkills.QuickFoul,        SkillCategory.Devious },
                { AllSkills.Saboteur,         SkillCategory.Devious },
                { AllSkills.Shadowing,        SkillCategory.Devious },
                { AllSkills.SneakyGit,        SkillCategory.Devious },
                { AllSkills.ViolentInnovator, SkillCategory.Devious },

                // General
                { AllSkills.Block,         SkillCategory.General },
                { AllSkills.Dauntless,     SkillCategory.General },
                { AllSkills.Fend,          SkillCategory.General },
                { AllSkills.Frenzy,        SkillCategory.General },
                { AllSkills.Kick,          SkillCategory.General },
                { AllSkills.Pro,           SkillCategory.General },
                { AllSkills.SteadyFooting, SkillCategory.General },
                { AllSkills.StripBall,     SkillCategory.General },
                { AllSkills.SureHands,     SkillCategory.General },
                { AllSkills.Tackle,        SkillCategory.General },
                { AllSkills.Taunt,         SkillCategory.General },
                { AllSkills.Wrestle,       SkillCategory.General },

                // Mutation
                { AllSkills.BigHand,           SkillCategory.Mutation },
                { AllSkills.Claw,              SkillCategory.Mutation },
                { AllSkills.DisturbingPresence, SkillCategory.Mutation },
                { AllSkills.ExtraArms,         SkillCategory.Mutation },
                { AllSkills.FoulAppearance,    SkillCategory.Mutation },
                { AllSkills.Horns,             SkillCategory.Mutation },
                { AllSkills.IronHardSkin,      SkillCategory.Mutation },
                { AllSkills.MonstrousMouth,    SkillCategory.Mutation },
                { AllSkills.PrehensileTail,    SkillCategory.Mutation },
                { AllSkills.Tentacles,         SkillCategory.Mutation },
                { AllSkills.TwoHeads,          SkillCategory.Mutation },
                { AllSkills.VeryLongLegs,      SkillCategory.Mutation },

                // Passing
                { AllSkills.Accurate,      SkillCategory.Passing },
                { AllSkills.Cannoneer,     SkillCategory.Passing },
                { AllSkills.CloudBurster,  SkillCategory.Passing },
                { AllSkills.DumpOff,       SkillCategory.Passing },
                { AllSkills.GiveAndGo,     SkillCategory.Passing },
                { AllSkills.HailMaryPass,  SkillCategory.Passing },
                { AllSkills.Leader,        SkillCategory.Passing },
                { AllSkills.NervesOfSteel, SkillCategory.Passing },
                { AllSkills.OnTheBall,     SkillCategory.Passing },
                { AllSkills.Pass,          SkillCategory.Passing },
                { AllSkills.Punt,          SkillCategory.Passing },
                { AllSkills.SafePass,      SkillCategory.Passing },

                // Strength
                { AllSkills.ArmBar,        SkillCategory.Strength },
                { AllSkills.Brawler,       SkillCategory.Strength },
                { AllSkills.BreakTackle,   SkillCategory.Strength },
                { AllSkills.Bullseye,      SkillCategory.Strength },
                { AllSkills.Grab,          SkillCategory.Strength },
                { AllSkills.Guard,         SkillCategory.Strength },
                { AllSkills.Juggernaut,    SkillCategory.Strength },
                { AllSkills.MightyBlow,    SkillCategory.Strength },
                { AllSkills.MultipleBlock, SkillCategory.Strength },
                { AllSkills.StandFirm,     SkillCategory.Strength },
                { AllSkills.StrongArm,     SkillCategory.Strength },
                { AllSkills.ThickSkull,    SkillCategory.Strength },
            };

        private static readonly IReadOnlyDictionary<AllSkills, CalculatorSkills> _toCalculatorMap =
            new Dictionary<AllSkills, CalculatorSkills>
            {
                { AllSkills.Brawler,     CalculatorSkills.Brawler     },
                { AllSkills.BreakTackle, CalculatorSkills.BreakTackle },
                { AllSkills.Catch,       CalculatorSkills.Catch       },
                { AllSkills.CloudBurster,CalculatorSkills.CloudBurster},
                { AllSkills.DirtyPlayer, CalculatorSkills.DirtyPlayer },
                { AllSkills.DivingCatch, CalculatorSkills.DivingCatch },
                { AllSkills.DivingTackle,CalculatorSkills.DivingTackle},
                { AllSkills.Dodge,       CalculatorSkills.Dodge       },
                { AllSkills.LoneFouler,  CalculatorSkills.LoneFouler  },
                { AllSkills.MightyBlow,  CalculatorSkills.MightyBlow  },
                { AllSkills.Pass,        CalculatorSkills.Pass        },
                { AllSkills.Pro,         CalculatorSkills.Pro         },
                { AllSkills.SneakyGit,   CalculatorSkills.SneakyGit   },
                { AllSkills.SureFeet,    CalculatorSkills.SureFeet    },
                { AllSkills.SureHands,   CalculatorSkills.SureHands   },
            };

        private static readonly IReadOnlyDictionary<CalculatorSkills, AllSkills> _fromCalculatorMap =
            _toCalculatorMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static SkillCategory GetCategory(AllSkills skill) => _categoryMap[skill];

        public static IReadOnlyList<AllSkills> GetByCategory(SkillCategory category) =>
            _categoryMap
                .Where(kvp => kvp.Value == category)
                .Select(kvp => kvp.Key)
                .ToList();

        public static CalculatorSkills? ToCalculatorSkills(AllSkills skill) =>
            _toCalculatorMap.TryGetValue(skill, out var cs) ? cs : (CalculatorSkills?)null;

        public static AllSkills? ToAllSkills(CalculatorSkills skill) =>
            _fromCalculatorMap.TryGetValue(skill, out var s) ? s : (AllSkills?)null;
    }
}
