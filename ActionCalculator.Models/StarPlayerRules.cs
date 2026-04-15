namespace ActionCalculator.Models
{
    public static class StarPlayerRules
    {
        public static IReadOnlyList<StarPlayerRule> All { get; } = new StarPlayerRule[]
        {
            new(StarPlayer.AkhorneTheSquirrel,          StarPlayerSkill.BlindRage,              ["Claws", "Dauntless", "Dodge", "Frenzy", "Jump Up", "Loner (4+)", "No Ball", "Sidestep", "Stunty", "Titchy"]),
            new(StarPlayer.AnqiPanqi,                   StarPlayerSkill.SavageBlow,             ["Block", "Grab", "Loner (4+)", "Stand Firm", "Unsteady"]),
            new(StarPlayer.BarikFarblast,               StarPlayerSkill.BlastIt,                ["Cannoneer", "Hail Mary Pass", "Loner (4+)", "Pass", "Secret Weapon", "Sure Hands", "Thick Skull"]),
            new(StarPlayer.BilerotVomitflesh,           StarPlayerSkill.PutridRegurgitation,    ["Dirty Player", "Disturbing Presence", "Foul Appearance", "Lone Fouler", "Loner (4+)", "Regeneration", "Unsteady"]),
            new(StarPlayer.BoaKonssstriktr,             StarPlayerSkill.LookIntoMyEyes,         ["Dodge", "Fend", "Hypnotic Gaze", "Loner (4+)", "Prehensile Tail", "Safe Pair Of Hands", "Sidestep"]),
            new(StarPlayer.BomberDribblesnot,           StarPlayerSkill.Kaboom,                 ["Accurate", "Bombardier", "Dodge", "Loner (4+)", "Right Stuff", "Secret Weapon", "Stunty"]),
            new(StarPlayer.CaptainKarinaVonRiesz,       StarPlayerSkill.TastyMorsel,            ["Bloodlust (2+)", "Dodge", "Hypnotic Gaze", "Jump Up", "Loner (4+)", "Regeneration"]),
            new(StarPlayer.CindyPiewhistle,             StarPlayerSkill.AllYouCanEat,           ["Accurate", "Bombardier", "Dodge", "Loner (4+)", "Secret Weapon", "Stunty"]),
            new(StarPlayer.CountLuthorVonDrakenborg,    StarPlayerSkill.StarOfTheShow,          ["Block", "Hypnotic Gaze", "Loner (4+)", "Regeneration", "Sidestep"]),
            new(StarPlayer.Crumbleberry,                StarPlayerSkill.IllCarryYou,            ["Dodge", "Lethal Flight", "Loner (4+)", "Right Stuff", "Stunty", "Sure Hands"]),
            new(StarPlayer.Dribl,                       StarPlayerSkill.ASneakyPair,            ["Dirty Player", "Dodge", "Loner (4+)", "Quick Foul", "Sidestep", "Sneaky Git", "Stunty"]),
            new(StarPlayer.Drull,                       StarPlayerSkill.ASneakyPair,            ["Dodge", "Loner (4+)", "Sidestep", "Stab", "Stunty"]),
            new(StarPlayer.EldrilSidewinder,            StarPlayerSkill.MesmerisingDance,       ["Catch", "Dodge", "Hypnotic Gaze", "Loner (4+)", "Nerves Of Steel", "On The Ball"]),
            new(StarPlayer.EstelleLaVeneaux,            StarPlayerSkill.BalefulHex,             ["Disturbing Presence", "Dodge", "Guard", "Loner (4+)", "Sidestep"]),
            new(StarPlayer.FrankNStein,                 StarPlayerSkill.BrutalBlock,            ["Break Tackle", "Loner (4+)", "Mighty Blow", "Regeneration", "Stand Firm", "Thick Skull"]),
            new(StarPlayer.FungusTheLoon,               StarPlayerSkill.WhirlingDervish,        ["Ball & Chain", "Loner (4+)", "Mighty Blow", "No Ball", "Secret Weapon", "Stunty"]),
            new(StarPlayer.GlartSmarshrip,              StarPlayerSkill.FrenziedRush,           ["Block", "Claws", "Grab", "Juggernaut", "Loner (4+)", "Stand Firm"]),
            new(StarPlayer.GlorielSummerbloom,          StarPlayerSkill.ShotToNothing,          ["Accurate", "Dodge", "Loner (3+)", "Pass", "Sidestep", "Sure Hands"]),
            new(StarPlayer.GlotlStop,                   StarPlayerSkill.PrimalSavagery,         ["Animal Savagery", "Frenzy", "Loner (4+)", "Mighty Blow", "Prehensile Tail", "Stand Firm", "Thick Skull"]),
            new(StarPlayer.Grak,                        StarPlayerSkill.IllCarryYou,            ["Bone Head", "Kick Team-mate", "Loner (4+)", "Mighty Blow", "Thick Skull"]),
            new(StarPlayer.GrashnakBlackhoof,           StarPlayerSkill.GoredByTheBull,         ["Frenzy", "Horns", "Loner (4+)", "Mighty Blow", "Thick Skull", "Unchannelled Fury"]),
            new(StarPlayer.GretchenWachter,             StarPlayerSkill.Incorporeal,            ["Disturbing Presence", "Dodge", "Foul Appearance", "Jump Up", "Loner (4+)", "No Ball", "Regeneration", "Shadowing", "Sidestep"]),
            new(StarPlayer.GriffOberwald,               StarPlayerSkill.ConsummateProfessional, ["Block", "Dodge", "Fend", "Loner (3+)", "Sprint", "Sure Feet"]),
            new(StarPlayer.GrombrindaTheWhiteDwarf,     StarPlayerSkill.WisdomOfTheWhiteDwarf,  ["Block", "Break Tackle", "Dauntless", "Loner (4+)", "Mighty Blow", "Stand Firm", "Sure Feet", "Thick Skull"]),
            new(StarPlayer.GufflePusmaw,                StarPlayerSkill.QuickBite,              ["Foul Appearance", "Loner (4+)", "Monstrous Mouth", "Nerves Of Steel", "On The Ball", "Plague Ridden"]),
            new(StarPlayer.HtharkTheUnstoppable,        StarPlayerSkill.UnstoppableMomentum,    ["Block", "Break Tackle", "Defensive", "Juggernaut", "Loner (4+)", "Sprint", "Sure Feet", "Thick Skull", "Unsteady"]),
            new(StarPlayer.HakflemSkuttlespike,         StarPlayerSkill.Treacherous,            ["Dodge", "Extra Arms", "Loner (4+)", "Prehensile Tail", "Two Heads"]),
            new(StarPlayer.HelmutWulf,                  StarPlayerSkill.OldPro,                 ["Chainsaw", "Loner (4+)", "No Ball", "Pro", "Secret Weapon", "Stand Firm"]),
            new(StarPlayer.IvanTheAnimalDeathshroud,    StarPlayerSkill.DwarfenScourge,         ["Block", "Disturbing Presence", "Hatred (Dwarf)", "Juggernaut", "Loner (4+)", "Regeneration", "Strip Ball", "Tackle"]),
            new(StarPlayer.IvarEriksson,                StarPlayerSkill.RaidingParty,           ["Block", "Guard", "Loner (4+)", "Tackle"]),
            new(StarPlayer.JeremiahKool,                StarPlayerSkill.TheFlashingBlade,       ["Block", "Diving Catch", "Dodge", "Dump-Off", "Loner (4+)", "Nerves Of Steel", "On The Ball", "Pass", "Sidestep"]),
            new(StarPlayer.JordellFreshbreeze,          StarPlayerSkill.SwiftAsTheBreeze,       ["Block", "Diving Catch", "Dodge", "Leap", "Loner (4+)", "Sidestep", "Steady Footing"]),
            new(StarPlayer.JosefBugman,                 StarPlayerSkill.DwarfenGrit,            ["Block", "Drunkard", "Fend", "Loner (3+)", "Tackle", "Taunt", "Thick Skull"]),
            new(StarPlayer.KarlaVonKill,                StarPlayerSkill.Indomitable,            ["Block", "Dauntless", "Dodge", "Jump Up", "Loner (4+)"]),
            new(StarPlayer.KirothKrakeneye,             StarPlayerSkill.BlackInk,               ["Disturbing Presence", "Foul Appearance", "Loner (4+)", "On The Ball", "Tackle", "Tentacles"]),
            new(StarPlayer.KreekTheVerminatorRustgouger,StarPlayerSkill.IllBeBack,              ["Ball & Chain", "Loner (4+)", "Mighty Blow", "No Ball", "Prehensile Tail", "Secret Weapon"]),
            new(StarPlayer.LordBorak,                   StarPlayerSkill.LordOfChaos,            ["Block", "Dirty Player", "Leader", "Loner (3+)", "Mighty Blow", "Put the Boot In", "Sneaky Git"]),
            new(StarPlayer.LucienSwift,                 StarPlayerSkill.WorkingInTandem,        ["Block", "Loner (4+)", "Mighty Blow", "Tackle"]),
            new(StarPlayer.MaxSpleenripper,             StarPlayerSkill.MaximumCarnage,         ["Chainsaw", "Loner (4+)", "No Ball", "Secret Weapon"]),
            new(StarPlayer.MightyZug,                   StarPlayerSkill.CrushingBlow,           ["Block", "Loner (4+)", "Mighty Blow", "Unsteady"]),
            new(StarPlayer.MorgNThorg,                  StarPlayerSkill.TheBallista,            ["Block", "Bullseye", "Hatred (Undead)", "Loner (4+)", "Mighty Blow", "Thick Skull", "Throw Team-mate"]),
            new(StarPlayer.NobblaBlackwart,             StarPlayerSkill.KickEmWhileTheyreDown,  ["Block", "Chainsaw", "Dodge", "Loner (4+)", "No Ball", "Saboteur", "Secret Weapon", "Stunty"]),
            new(StarPlayer.PuggyBaconbreath,            StarPlayerSkill.HalflingLuck,           ["Block", "Dodge", "Loner (3+)", "Nerves Of Steel", "Right Stuff", "Stunty"]),
            new(StarPlayer.RashnackBackstabber,         StarPlayerSkill.ToxinConnoisseur,       ["Loner (4+)", "Shadowing", "Sidestep", "Sneaky Git", "Stab"]),
            new(StarPlayer.RipperBolgrot,               StarPlayerSkill.ThinkingMansTroll,      ["Bullseye", "Grab", "Loner (4+)", "Mighty Blow", "Regeneration", "Throw Team-mate"]),
            new(StarPlayer.RoxannaDarknail,             StarPlayerSkill.SlashingNails,          ["Dodge", "Frenzy", "Jump Up", "Juggernaut", "Leap", "Loner (4+)"]),
            new(StarPlayer.RumbelowSheepskin,           StarPlayerSkill.Ram,                    ["Block", "Horns", "Juggernaut", "Loner (4+)", "Tackle", "Thick Skull"]),
            new(StarPlayer.ScylaAnfingrimm,             StarPlayerSkill.FuryOfTheBloodGod,      ["Claws", "Frenzy", "Loner (4+)", "Mighty Blow", "Prehensile Tail", "Thick Skull", "Unchannelled Fury"]),
            new(StarPlayer.ScrappaSorehead,             StarPlayerSkill.Yoink,                  ["Dirty Player", "Dodge", "Loner (4+)", "Pogo", "Right Stuff", "Sprint", "Stunty", "Sure Feet"]),
            new(StarPlayer.SkitterStabStab,             StarPlayerSkill.MasterAssassin,         ["Dodge", "Loner (4+)", "Prehensile Tail", "Shadowing", "Stab"]),
            new(StarPlayer.SkrorgSnowpelt,              StarPlayerSkill.PumpUpTheCrowd,         ["Block", "Claws", "Disturbing Presence", "Juggernaut", "Loner (4+)", "Mighty Blow"]),
            new(StarPlayer.SkrullHalfheight,            StarPlayerSkill.StrongPassingGame,      ["Accurate", "Loner (4+)", "Nerves Of Steel", "Pass", "Regeneration", "Sure Hands", "Thick Skull"]),
            new(StarPlayer.TheBlackGobbo,               StarPlayerSkill.SneakiestOfTheLot,      ["Bombardier", "Disturbing Presence", "Dodge", "Loner (3+)", "Sidestep", "Sneaky Git", "Stab", "Stunty"]),
            new(StarPlayer.ThorssonStoutmead,           StarPlayerSkill.BeerBarrelBash,         ["Block", "Drunkard", "Loner (4+)", "Thick Skull"]),
            new(StarPlayer.ValenSwift,                  StarPlayerSkill.WorkingInTandem,        ["Accurate", "Loner (4+)", "Nerves Of Steel", "Pass", "Safe Pass", "Sure Hands"]),
            new(StarPlayer.VaragGhoulChewer,            StarPlayerSkill.KrumpAndSmash,          ["Block", "Hatred (Undead)", "Jump Up", "Loner (4+)", "Mighty Blow", "Thick Skull", "Unsteady"]),
            new(StarPlayer.WilhelmChaney,               StarPlayerSkill.SavageMauling,          ["Catch", "Claws", "Frenzy", "Loner (4+)", "Regeneration", "Wrestle"]),
            new(StarPlayer.WithergraspDoubledrool,      StarPlayerSkill.WatchOut,               ["Foul Appearance", "Loner (4+)", "Prehensile Tail", "Tackle", "Tentacles", "Two Heads", "Wrestle"]),
            new(StarPlayer.ZolcathTheZoat,              StarPlayerSkill.ExcuseMeAreYouAZoat,    ["Disturbing Presence", "Juggernaut", "Loner (4+)", "Mighty Blow", "Prehensile Tail", "Regeneration", "Sure Feet"]),
        };

        public static IReadOnlyDictionary<StarPlayer, StarPlayerRule> ByStarPlayer { get; } =
            All.ToDictionary(r => r.StarPlayer);

        public static IReadOnlyDictionary<StarPlayerSkill, StarPlayerRule> BySkill { get; } =
            All.GroupBy(r => r.Skill)
               .ToDictionary(g => g.Key, g => g.First());
    }
}
