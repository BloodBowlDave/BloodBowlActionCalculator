namespace ActionCalculator.Models
{
    public static class StarPlayerRules
    {
        public static IReadOnlyList<StarPlayerRule> All { get; } = new StarPlayerRule[]
        {
            new(StarPlayer.AkhorneTheSquirrel,          StarPlayerSkill.BlindRage,              ["Claws", "Dauntless", "Dodge", "Frenzy", "Jump Up", "Loner (4+)", "No Ball", "Sidestep", "Stunty", "Titchy"],                                                          "D,CL,L4,BR"),
            new(StarPlayer.AnqiPanqi,                   StarPlayerSkill.SavageBlow,             ["Block", "Grab", "Loner (4+)", "Stand Firm", "Unsteady"],                                                                                                              "L4,SB"),
            new(StarPlayer.BarikFarblast,               StarPlayerSkill.BlastIt,                ["Cannoneer", "Hail Mary Pass", "Loner (4+)", "Pass", "Secret Weapon", "Sure Hands", "Thick Skull"],                                                                    "SH,PA,L4,BI"),
            new(StarPlayer.BilerotVomitflesh,           StarPlayerSkill.PutridRegurgitation,    ["Dirty Player", "Disturbing Presence", "Foul Appearance", "Lone Fouler", "Loner (4+)", "Regeneration", "Unsteady"],                                                    "L4,DP1,LF"),
            new(StarPlayer.BoaKonssstriktr,             StarPlayerSkill.LookIntoMyEyes,         ["Dodge", "Fend", "Hypnotic Gaze", "Loner (4+)", "Prehensile Tail", "Safe Pair Of Hands", "Sidestep"],                                                                  "D,L4"),
            new(StarPlayer.BomberDribblesnot,           StarPlayerSkill.Kaboom,                 ["Accurate", "Bombardier", "Dodge", "Loner (4+)", "Right Stuff", "Secret Weapon", "Stunty"],                                                                            "D,L4"),
            new(StarPlayer.CaptainKarinaVonRiesz,       StarPlayerSkill.TastyMorsel,            ["Bloodlust (2+)", "Dodge", "Hypnotic Gaze", "Jump Up", "Loner (4+)", "Regeneration"],                                                                                  "D,L4"),
            new(StarPlayer.CindyPiewhistle,             StarPlayerSkill.AllYouCanEat,           ["Accurate", "Bombardier", "Dodge", "Loner (4+)", "Secret Weapon", "Stunty"],                                                                                           "D,L4"),
            new(StarPlayer.CountLuthorVonDrakenborg,    StarPlayerSkill.StarOfTheShow,          ["Block", "Hypnotic Gaze", "Loner (4+)", "Regeneration", "Sidestep"],                                                                                                   "L4"),
            new(StarPlayer.Crumbleberry,                StarPlayerSkill.IllCarryYou,            ["Dodge", "Lethal Flight", "Loner (4+)", "Right Stuff", "Stunty", "Sure Hands"],                                                                                        "D,SH,L4"),
            new(StarPlayer.DeeprootStrongbranch,        StarPlayerSkill.Reliable,               ["Block", "Bullseye", "Loner (4+)", "Mighty Blow", "Stand Firm", "Strong Arm", "Thick Skull", "Throw Team-mate", "Timmm-ber!"],                                              "MB,L4"),
            new(StarPlayer.Dribl,                       StarPlayerSkill.ASneakyPair,            ["Dirty Player", "Dodge", "Loner (4+)", "Quick Foul", "Sidestep", "Sneaky Git", "Stunty"],                                                                              "D,L4,DP1,SG,ASP"),
            new(StarPlayer.Drull,                       StarPlayerSkill.ASneakyPair,            ["Dodge", "Loner (4+)", "Sidestep", "Stab", "Stunty"],                                                                                                                  "D,L4,ASP"),
            new(StarPlayer.EldrilSidewinder,            StarPlayerSkill.MesmerisingDance,       ["Catch", "Dodge", "Hypnotic Gaze", "Loner (4+)", "Nerves Of Steel", "On The Ball"],                                                                                    "D,C,L4,MD"),
            new(StarPlayer.EstelleLaVeneaux,            StarPlayerSkill.BalefulHex,             ["Disturbing Presence", "Dodge", "Guard", "Loner (4+)", "Sidestep"],                                                                                                    "D,L4"),
            new(StarPlayer.FrankNStein,                 StarPlayerSkill.BrutalBlock,            ["Break Tackle", "Loner (4+)", "Mighty Blow", "Regeneration", "Stand Firm", "Thick Skull"],                                                                             "BT,MB,L4,BB"),
            new(StarPlayer.FungusTheLoon,               StarPlayerSkill.WhirlingDervish,        ["Ball & Chain", "Loner (4+)", "Mighty Blow", "No Ball", "Secret Weapon", "Stunty"],                                                                                    "MB,L4,WD"),
            new(StarPlayer.GlartSmashrip,              StarPlayerSkill.FrenziedRush,           ["Block", "Claws", "Grab", "Juggernaut", "Loner (4+)", "Stand Firm"],                                                                                                   "CL,L4"),
            new(StarPlayer.GlorielSummerbloom,          StarPlayerSkill.ShotToNothing,          ["Accurate", "Dodge", "Loner (3+)", "Pass", "Sidestep", "Sure Hands"],                                                                                                  "D,SH,PA,L3"),
            new(StarPlayer.GlotlStop,                   StarPlayerSkill.PrimalSavagery,         ["Animal Savagery", "Frenzy", "Loner (4+)", "Mighty Blow", "Prehensile Tail", "Stand Firm", "Thick Skull"],                                                             "MB,L4"),
            new(StarPlayer.Grak,                        StarPlayerSkill.IllCarryYou,            ["Bone Head", "Kick Team-mate", "Loner (4+)", "Mighty Blow", "Thick Skull"],                                                                                            "MB,L4"),
            new(StarPlayer.GrashnakBlackhoof,           StarPlayerSkill.GoredByTheBull,         ["Frenzy", "Horns", "Loner (4+)", "Mighty Blow", "Thick Skull", "Unchannelled Fury"],                                                                                   "MB,L4"),
            new(StarPlayer.GretchenWachter,             StarPlayerSkill.Incorporeal,            ["Disturbing Presence", "Dodge", "Foul Appearance", "Jump Up", "Loner (4+)", "No Ball", "Regeneration", "Shadowing", "Sidestep"],                                       "D,L4"),
            new(StarPlayer.GrimIronjaw,                 StarPlayerSkill.Slayer,                 ["Block", "Dauntless", "Frenzy", "Hatred (Big Guy)", "Loner (4+)", "Multiple Block", "Thick Skull"],                                                                       "L4,S"),
            new(StarPlayer.GriffOberwald,               StarPlayerSkill.ConsummateProfessional, ["Block", "Dodge", "Fend", "Loner (3+)", "Sprint", "Sure Feet"],                                                                                                        "D,SF,L3,CP"),
            new(StarPlayer.GrombrindaTheWhiteDwarf,     StarPlayerSkill.WisdomOfTheWhiteDwarf,  ["Block", "Break Tackle", "Dauntless", "Loner (4+)", "Mighty Blow", "Stand Firm", "Sure Feet", "Thick Skull"],                                                          "BT,SF,MB,L4"),
            new(StarPlayer.GufflePusmaw,                StarPlayerSkill.QuickBite,              ["Foul Appearance", "Loner (4+)", "Monstrous Mouth", "Nerves Of Steel", "On The Ball", "Plague Ridden"],                                                                "L4"),
            new(StarPlayer.HtharkTheUnstoppable,        StarPlayerSkill.UnstoppableMomentum,    ["Block", "Break Tackle", "Defensive", "Juggernaut", "Loner (4+)", "Sprint", "Sure Feet", "Thick Skull", "Unsteady"],                                                   "BT,SF,L4,UM"),
            new(StarPlayer.HakflemSkuttlespike,         StarPlayerSkill.Treacherous,            ["Dodge", "Extra Arms", "Loner (4+)", "Prehensile Tail", "Two Heads"],                                                                                                  "D,L4"),
            new(StarPlayer.HelmutWulf,                  StarPlayerSkill.OldPro,                 ["Chainsaw", "Loner (4+)", "No Ball", "Pro", "Secret Weapon", "Stand Firm"],                                                                                            "P,L4,OP"),
            new(StarPlayer.IvanTheAnimalDeathshroud,    StarPlayerSkill.DwarfenScourge,         ["Block", "Disturbing Presence", "Hatred (Dwarf)", "Juggernaut", "Loner (4+)", "Regeneration", "Strip Ball", "Tackle"],                                                 "L4,DS,H"),
            new(StarPlayer.IvarEriksson,                StarPlayerSkill.RaidingParty,           ["Block", "Guard", "Loner (4+)", "Tackle"],                                                                                                                             "L4"),
            new(StarPlayer.JeremiahKool,                StarPlayerSkill.TheFlashingBlade,       ["Block", "Diving Catch", "Dodge", "Dump-Off", "Loner (4+)", "Nerves Of Steel", "On The Ball", "Pass", "Sidestep"],                                                     "D,PA,DC,L4"),
            new(StarPlayer.JordellFreshbreeze,          StarPlayerSkill.SwiftAsTheBreeze,       ["Block", "Diving Catch", "Dodge", "Leap", "Loner (4+)", "Sidestep", "Steady Footing"],                                                                                 "D,DC,L4"),
            new(StarPlayer.JosefBugman,                 StarPlayerSkill.DwarfenGrit,            ["Block", "Drunkard", "Fend", "Loner (3+)", "Tackle", "Taunt", "Thick Skull"],                                                                                         "L3"),
            new(StarPlayer.KarlaVonKill,                StarPlayerSkill.Indomitable,            ["Block", "Dauntless", "Dodge", "Jump Up", "Loner (4+)"],                                                                                                              "D,L4"),
            new(StarPlayer.KirothKrakeneye,             StarPlayerSkill.BlackInk,               ["Disturbing Presence", "Foul Appearance", "Loner (4+)", "On The Ball", "Tackle", "Tentacles"],                                                                         "L4"),
            new(StarPlayer.KreekTheVerminatorRustgouger,StarPlayerSkill.IllBeBack,              ["Ball & Chain", "Loner (4+)", "Mighty Blow", "No Ball", "Prehensile Tail", "Secret Weapon"],                                                                           "MB,L4"),
            new(StarPlayer.LordBorak,                   StarPlayerSkill.LordOfChaos,            ["Block", "Dirty Player", "Leader", "Loner (3+)", "Mighty Blow", "Put the Boot In", "Sneaky Git"],                                                                      "MB,L3,DP1,SG,LC"),
            new(StarPlayer.LucienSwift,                 StarPlayerSkill.WorkingInTandem,        ["Block", "Loner (4+)", "Mighty Blow", "Tackle"],                                                                                                                       "MB,L4"),
            new(StarPlayer.MapleHighgrove,              StarPlayerSkill.ViciousVines,           ["Brawler", "Grab", "Loner (4+)", "Mighty Blow", "Stand Firm", "Tentacles", "Thick Skull"],                                                                                "MB,B,L4"),
            new(StarPlayer.MaxSpleenripper,             StarPlayerSkill.MaximumCarnage,         ["Chainsaw", "Loner (4+)", "No Ball", "Secret Weapon"],                                                                                                                 "L4"),
            new(StarPlayer.MightyZug,                   StarPlayerSkill.CrushingBlow,           ["Block", "Loner (4+)", "Mighty Blow", "Unsteady"],                                                                                                                     "MB,L4,CR"),
            new(StarPlayer.MorgNThorg,                  StarPlayerSkill.TheBallista,            ["Block", "Bullseye", "Hatred (Undead)", "Loner (4+)", "Mighty Blow", "Thick Skull", "Throw Team-mate"],                                                                "MB,L4,TB,H"),
            new(StarPlayer.NobblaBlackwart,             StarPlayerSkill.KickEmWhileTheyreDown,  ["Block", "Chainsaw", "Dodge", "Loner (4+)", "No Ball", "Saboteur", "Secret Weapon", "Stunty"],                                                                         "D,L4"),
            new(StarPlayer.PuggyBaconbreath,            StarPlayerSkill.HalflingLuck,           ["Block", "Dodge", "Loner (3+)", "Nerves Of Steel", "Right Stuff", "Stunty"],                                                                                           "D,L3,HL"),
            new(StarPlayer.RashnackBackstabber,         StarPlayerSkill.ToxinConnoisseur,       ["Loner (4+)", "Shadowing", "Sidestep", "Sneaky Git", "Stab"],                                                                                                          "L4,SG,TC"),
            new(StarPlayer.RipperBolgrot,               StarPlayerSkill.ThinkingMansTroll,      ["Bullseye", "Grab", "Loner (4+)", "Mighty Blow", "Regeneration", "Throw Team-mate"],                                                                                   "MB,L4,TMT"),
            new(StarPlayer.RodneyRoachbait,             StarPlayerSkill.CatchOfTheDay,          ["Catch", "Diving Catch", "Jump Up", "Loner (4+)", "On the Ball", "Sidestep", "Stunty", "Wrestle"],                                                                        "C,DC,L4"),
            new(StarPlayer.RowanaForestfoot,            StarPlayerSkill.BoundingLeap,           ["Dodge", "Dump-Off", "Guard", "Horns", "Jump Up", "Leap", "Loner (4+)"],                                                                                                   "D,L4,BL"),
            new(StarPlayer.RoxannaDarknail,             StarPlayerSkill.SlashingNails,          ["Dodge", "Frenzy", "Jump Up", "Juggernaut", "Leap", "Loner (4+)"],                                                                                                     "D,L4"),
            new(StarPlayer.RumbelowSheepskin,           StarPlayerSkill.Ram,                    ["Block", "Horns", "Juggernaut", "Loner (4+)", "Tackle", "Thick Skull"],                                                                                                "L4,R"),
            new(StarPlayer.ScylaAnfingrimm,             StarPlayerSkill.FuryOfTheBloodGod,      ["Claws", "Frenzy", "Loner (4+)", "Mighty Blow", "Prehensile Tail", "Thick Skull", "Unchannelled Fury"],                                                                "CL,MB,L4"),
            new(StarPlayer.ScrappaSorehead,             StarPlayerSkill.Yoink,                  ["Dirty Player", "Dodge", "Loner (4+)", "Pogo", "Right Stuff", "Sprint", "Stunty", "Sure Feet"],                                                                        "D,SF,L4,DP1"),
            new(StarPlayer.SkitterStabStab,             StarPlayerSkill.MasterAssassin,         ["Dodge", "Loner (4+)", "Prehensile Tail", "Shadowing", "Stab"],                                                                                                        "D,L4,MA"),
            new(StarPlayer.SkrorgSnowpelt,              StarPlayerSkill.PumpUpTheCrowd,         ["Block", "Claws", "Disturbing Presence", "Juggernaut", "Loner (4+)", "Mighty Blow"],                                                                                   "CL,MB,L4"),
            new(StarPlayer.SkrullHalfheight,            StarPlayerSkill.StrongPassingGame,      ["Accurate", "Loner (4+)", "Nerves Of Steel", "Pass", "Regeneration", "Sure Hands", "Thick Skull"],                                                                     "SH,PA,L4"),
            new(StarPlayer.SwiftvineGlimmershard,       StarPlayerSkill.FuriousOutburst,        ["Disturbing Presence", "Fend", "Loner (4+)", "Sidestep", "Stab", "Stunty"],                                                                                              "L4"),
            new(StarPlayer.TheBlackGobbo,               StarPlayerSkill.SneakiestOfTheLot,      ["Bombardier", "Disturbing Presence", "Dodge", "Loner (3+)", "Sidestep", "Sneaky Git", "Stab", "Stunty"],                                                              "D,L3,SG"),
            new(StarPlayer.ThorssonStoutmead,           StarPlayerSkill.BeerBarrelBash,         ["Block", "Drunkard", "Loner (4+)", "Thick Skull"],                                                                                                                     "L4"),
            new(StarPlayer.ValenSwift,                  StarPlayerSkill.WorkingInTandem,        ["Accurate", "Loner (4+)", "Nerves Of Steel", "Pass", "Safe Pass", "Sure Hands"],                                                                                       "SH,PA,L4"),
            new(StarPlayer.VaragGhoulChewer,            StarPlayerSkill.KrumpAndSmash,          ["Block", "Hatred (Undead)", "Jump Up", "Loner (4+)", "Mighty Blow", "Thick Skull", "Unsteady"],                                                                        "MB,L4,H"),
            new(StarPlayer.WilhelmChaney,               StarPlayerSkill.SavageMauling,          ["Catch", "Claws", "Frenzy", "Loner (4+)", "Regeneration", "Wrestle"],                                                                                                 "C,CL,L4,SM"),
            new(StarPlayer.WillowRosebark,              StarPlayerSkill.WoodlandFury,           ["Dauntless", "Loner (4+)", "Sidestep", "Thick Skull"],                                                                                                                   "L4,WF"),
            new(StarPlayer.WithergraspDoubledrool,      StarPlayerSkill.WatchOut,               ["Foul Appearance", "Loner (4+)", "Prehensile Tail", "Tackle", "Tentacles", "Two Heads", "Wrestle"],                                                                    "L4"),
            new(StarPlayer.ZolcathTheZoat,              StarPlayerSkill.ExcuseMeAreYouAZoat,    ["Disturbing Presence", "Juggernaut", "Loner (4+)", "Mighty Blow", "Prehensile Tail", "Regeneration", "Sure Feet"],                                                     "SF,MB,L4"),
            new(StarPlayer.ZzhargMadeye,                StarPlayerSkill.BlastinSolvesEverything,["Cannoneer", "Hail Mary Pass", "Loner (4+)", "Nerves of Steel", "Secret Weapon", "Thick Skull"],                                                                         "L4"),
        };

        public static IReadOnlyDictionary<StarPlayer, StarPlayerRule> ByStarPlayer { get; } =
            All.ToDictionary(r => r.StarPlayer);

        public static IReadOnlyDictionary<StarPlayerSkill, StarPlayerRule> BySkill { get; } =
            All.GroupBy(r => r.Skill)
               .ToDictionary(g => g.Key, g => g.First());

        public static IReadOnlyDictionary<StarPlayerSkill, CalculatorSkills> SkillMapping { get; } =
            Enum.GetValues<StarPlayerSkill>()
                .Select(s => (starPlayerSkill: s, calculatorSkill: Enum.TryParse<CalculatorSkills>(s.ToString(), out var cs) ? cs : (CalculatorSkills?)null))
                .Where(x => x.calculatorSkill.HasValue)
                .ToDictionary(x => x.starPlayerSkill, x => x.calculatorSkill!.Value);

        public static IReadOnlyDictionary<StarPlayer, string> ShortNameByStarPlayer { get; } =
            All.ToDictionary(
                r => r.StarPlayer,
                r => typeof(StarPlayer)
                    .GetField(r.StarPlayer.ToString())!
                    .GetCustomAttributes(typeof(ShortNameAttribute), false)
                    .Cast<ShortNameAttribute>()
                    .First().Name);

        public static IReadOnlyDictionary<string, StarPlayerRule> ByShortName { get; } = BuildByShortName();

        private static Dictionary<string, StarPlayerRule> BuildByShortName()
        {
            var dict = All.ToDictionary(
                r => typeof(StarPlayer)
                    .GetField(r.StarPlayer.ToString())!
                    .GetCustomAttributes(typeof(ShortNameAttribute), false)
                    .Cast<ShortNameAttribute>()
                    .First().Name,
                StringComparer.OrdinalIgnoreCase);

            var ivan = All.First(r => r.StarPlayer == StarPlayer.IvanTheAnimalDeathshroud);
            dict["Ivan*"] = ivan;

            return dict;
        }
    }
}
