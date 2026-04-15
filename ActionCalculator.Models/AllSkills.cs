using System.ComponentModel;

namespace ActionCalculator.Models
{
    public enum AllSkills
    {
        // Agility
        [Description("Catch")]             Catch = 1,
        [Description("Defensive")]         Defensive,
        [Description("Diving Catch")]      DivingCatch,
        [Description("Diving Tackle")]     DivingTackle,
        [Description("Dodge")]             Dodge,
        [Description("Hit and Run")]       HitAndRun,
        [Description("Jump Up")]           JumpUp,
        [Description("Leap")]              Leap,
        [Description("Safe Pair of Hands")]SafePairOfHands,
        [Description("Sidestep")]          Sidestep,
        [Description("Sprint")]            Sprint,
        [Description("Sure Feet")]         SureFeet,

        // Devious
        [Description("Dirty Player")]      DirtyPlayer,
        [Description("Eye Gouge")]         EyeGouge,
        [Description("Fumblerooski")]      Fumblerooski,
        [Description("Lethal Flight")]     LethalFlight,
        [Description("Lone Fouler")]       LoneFouler,
        [Description("Pile Driver")]       PileDriver,
        [Description("Put the Boot In")]   PutTheBootIn,
        [Description("Quick Foul")]        QuickFoul,
        [Description("Saboteur")]          Saboteur,
        [Description("Shadowing")]         Shadowing,
        [Description("Sneaky Git")]        SneakyGit,
        [Description("Violent Innovator")] ViolentInnovator,

        // General
        [Description("Block")]             Block,
        [Description("Dauntless")]         Dauntless,
        [Description("Fend")]              Fend,
        [Description("Frenzy")]            Frenzy,
        [Description("Kick")]              Kick,
        [Description("Pro")]               Pro,
        [Description("Steady Footing")]    SteadyFooting,
        [Description("Strip Ball")]        StripBall,
        [Description("Sure Hands")]        SureHands,
        [Description("Tackle")]            Tackle,
        [Description("Taunt")]             Taunt,
        [Description("Wrestle")]           Wrestle,

        // Mutation
        [Description("Big Hand")]          BigHand,
        [Description("Claw")]              Claw,
        [Description("Disturbing Presence")]DisturbingPresence,
        [Description("Extra Arms")]        ExtraArms,
        [Description("Foul Appearance")]   FoulAppearance,
        [Description("Horns")]             Horns,
        [Description("Iron Hard Skin")]    IronHardSkin,
        [Description("Monstrous Mouth")]   MonstrousMouth,
        [Description("Prehensile Tail")]   PrehensileTail,
        [Description("Tentacles")]         Tentacles,
        [Description("Two Heads")]         TwoHeads,
        [Description("Very Long Legs")]    VeryLongLegs,

        // Passing
        [Description("Accurate")]          Accurate,
        [Description("Cannoneer")]         Cannoneer,
        [Description("Cloud Burster")]     CloudBurster,
        [Description("Dump-Off")]          DumpOff,
        [Description("Give and Go")]       GiveAndGo,
        [Description("Hail Mary Pass")]    HailMaryPass,
        [Description("Leader")]            Leader,
        [Description("Nerves of Steel")]   NervesOfSteel,
        [Description("On the Ball")]       OnTheBall,
        [Description("Pass")]              Pass,
        [Description("Punt")]              Punt,
        [Description("Safe Pass")]         SafePass,

        // Strength
        [Description("Arm Bar")]           ArmBar,
        [Description("Brawler")]           Brawler,
        [Description("Break Tackle")]      BreakTackle,
        [Description("Bullseye")]          Bullseye,
        [Description("Grab")]              Grab,
        [Description("Guard")]             Guard,
        [Description("Juggernaut")]        Juggernaut,
        [Description("Mighty Blow")]       MightyBlow,
        [Description("Multiple Block")]    MultipleBlock,
        [Description("Stand Firm")]        StandFirm,
        [Description("Strong Arm")]        StrongArm,
        [Description("Thick Skull")]       ThickSkull,
    }
}
